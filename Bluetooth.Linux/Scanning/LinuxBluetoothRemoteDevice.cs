using Bluetooth.Linux.Scanning.Factories;
using ConnectionOptions = Bluetooth.Abstractions.Scanning.Options.ConnectionOptions;

namespace Bluetooth.Linux.Scanning;

/// <summary>
///     Linux implementation of a remote BLE device backed by a BlueZ D-Bus <see cref="Device"/> proxy.
/// </summary>
public class LinuxBluetoothRemoteDevice : BaseBluetoothRemoteDevice
{
    private Device? _nativeDevice;

    /// <inheritdoc />
    public LinuxBluetoothRemoteDevice(
        IBluetoothScanner scanner,
        IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec,
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothRemoteDevice>? logger = null)
        : base(scanner, spec, serviceFactory, rssiToSignalStrengthConverter, logger)
    {
        if (spec is LinuxBluetoothRemoteDeviceFactorySpec linuxSpec)
        {
            _nativeDevice = linuxSpec.NativeDevice;
        }
    }

    private Device RequireNativeDevice() =>
        _nativeDevice ?? throw new InvalidOperationException(
            $"No native BlueZ Device for '{Id}'. The device must be discovered by the scanner before connecting.");

    #region Connection

    /// <inheritdoc />
    protected override void NativeRefreshIsConnected()
    {
        if (_nativeDevice == null)
        {
            IsConnected = false;
            return;
        }

        // Fire-and-forget async refresh; IsConnected will be updated by connection/disconnection events
        _ = RefreshIsConnectedAsync();
    }

    private async Task RefreshIsConnectedAsync()
    {
        try
        {
            var props = await _nativeDevice!.GetAllAsync().ConfigureAwait(false);
            IsConnected = props.Connected;
        }
        catch
        {
            IsConnected = false;
        }
    }

    /// <inheritdoc />
    protected override async ValueTask NativeConnectAsync(ConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Connecting to {DeviceId} via BlueZ", Id);

        var device = RequireNativeDevice();

        try
        {
            // Subscribe to connection and services-resolved events before connecting
            device.Connected += OnBlueZConnected;
            device.Disconnected += OnBlueZDisconnected;
            device.ServicesResolved += OnBlueZServicesResolved;

            await device.ConnectAsync().ConfigureAwait(false);

            // Wait for ServicesResolved so GATT is ready when ConnectAsync returns to the caller
            using var resolvedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (timeout.HasValue)
                resolvedCts.CancelAfter(timeout.Value);

            await WaitForPropertyToBeOfValue(nameof(IsConnected), true, timeout, resolvedCts.Token).ConfigureAwait(false);

            Logger?.LogDebug("Connected to {DeviceId}", Id);
        }
        catch (Exception ex)
        {
            device.Connected -= OnBlueZConnected;
            device.Disconnected -= OnBlueZDisconnected;
            device.ServicesResolved -= OnBlueZServicesResolved;
            OnConnectFailed(ex);
            throw;
        }
    }

    /// <inheritdoc />
    protected override async ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Disconnecting from {DeviceId}", Id);

        if (_nativeDevice == null)
            return;

        try
        {
            await _nativeDevice.DisconnectAsync().ConfigureAwait(false);
            Logger?.LogDebug("Disconnected from {DeviceId}", Id);
        }
        finally
        {
            _nativeDevice.Connected -= OnBlueZConnected;
            _nativeDevice.Disconnected -= OnBlueZDisconnected;
            _nativeDevice.ServicesResolved -= OnBlueZServicesResolved;
        }
    }

    #endregion

    #region BlueZ event callbacks

    private Task OnBlueZConnected(Device sender, BlueZEventArgs e)
    {
        if (!e.IsStateChange)
            return Task.CompletedTask;

        IsConnected = true;
        OnConnectSucceeded();
        return Task.CompletedTask;
    }

    private Task OnBlueZDisconnected(Device sender, BlueZEventArgs e)
    {
        if (!e.IsStateChange)
            return Task.CompletedTask;

        IsConnected = false;
        OnDisconnect();
        return Task.CompletedTask;
    }

    private Task OnBlueZServicesResolved(Device sender, BlueZEventArgs e)
    {
        // Services are now available; the base class handles caching after ExploreServicesAsync
        return Task.CompletedTask;
    }

    #endregion

    #region Service discovery

    /// <inheritdoc />
    protected override async ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Exploring services on {DeviceId}", Id);

        try
        {
            var device = RequireNativeDevice();
            var nativeServices = await device.GetServicesAsync().ConfigureAwait(false);

            // Pre-fetch UUIDs asynchronously before entering the synchronous conversion lambda
            var serviceEntries = await Task.WhenAll(nativeServices.Select(async svc =>
            {
                var uuid = await svc.GetUUIDAsync().ConfigureAwait(false);
                var normalized = BlueZManager.NormalizeUUID(uuid);
                return (Native: svc, Id: Guid.TryParse(normalized, out var g) ? g : Guid.Empty);
            })).ConfigureAwait(false);

            var validEntries = serviceEntries.Where(e => e.Id != Guid.Empty).ToList();

            Logger?.LogDebug("Found {Count} services on {DeviceId}", validEntries.Count, Id);

            OnServicesExplorationSucceeded(
                validEntries,
                areRepresentingTheSameObject: (entry, shared) => entry.Id == shared.Id,
                fromInputTypeToOutputTypeConversion: entry =>
                {
                    var spec = new LinuxBluetoothRemoteServiceFactorySpec(entry.Id, entry.Native);
                    return (ServiceFactory ?? throw new InvalidOperationException("ServiceFactory must be set")).Create(this, spec);
                });
        }
        catch (Exception ex)
        {
            Logger?.LogDebug(ex, "Service exploration failed on {DeviceId}", Id);
            OnServicesExplorationFailed(ex);
        }
    }

    #endregion

    #region Unsupported native operations

    /// <inheritdoc />
    protected override ValueTask NativeRequestMtuAsync(int requestedMtu) =>
        throw new NotSupportedException("MTU negotiation is not directly supported on Linux via BlueZ D-Bus.");

    /// <inheritdoc />
    protected override ValueTask NativeRequestConnectionPriorityAsync(ConnectionPriority priority, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("Connection priority requests are not supported on Linux.");

    /// <inheritdoc />
    protected override ValueTask NativeOpenL2CapChannelAsync(int psm) =>
        throw new NotSupportedException("L2CAP CoC channels are not yet supported on Linux.");

    /// <inheritdoc />
    protected override ValueTask NativeSetPreferredPhyAsync(PhyMode txPhy, PhyMode rxPhy) =>
        throw new NotSupportedException("PHY preference settings are not supported on Linux.");

    /// <inheritdoc />
    protected override void NativeReadSignalStrength()
    {
        // RSSI is only available during scanning on Linux; fire-and-forget attempt
        _ = ReadSignalStrengthAsync();
    }

    private async Task ReadSignalStrengthAsync()
    {
        try
        {
            if (_nativeDevice == null)
                return;

            var props = await _nativeDevice.GetAllAsync().ConfigureAwait(false);
            OnSignalStrengthRead(props.RSSI);
        }
        catch
        {
            // RSSI may not be available for connected devices on Linux
        }
    }

    #endregion
}
