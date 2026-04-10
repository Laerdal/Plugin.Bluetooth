using Bluetooth.Abstractions.Scanning.Options;
using Bluetooth.Abstractions.Options;
using Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <summary>
///     Linux BLE remote device backed by a BlueZ <c>org.bluez.Device1</c> D-Bus object.
/// </summary>
public class LinuxBluetoothRemoteDevice : BaseBluetoothRemoteDevice, IAsyncDisposable
{
    private readonly string _devicePath;
    private readonly LinuxBluetoothAdapter _adapter;
    private IDisposable? _propertiesChangedSubscription;

    /// <inheritdoc />
    public LinuxBluetoothRemoteDevice(
        IBluetoothScanner parentScanner,
        IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec,
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        LinuxBluetoothAdapter adapter,
        ILogger<IBluetoothRemoteDevice>? logger = null)
        : base(parentScanner, spec, serviceFactory, rssiToSignalStrengthConverter, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);
        ArgumentNullException.ThrowIfNull(adapter);

        _adapter = adapter;

        // Resolve the device path from the spec (Linux-specific spec carries ObjectPath).
        _devicePath = spec is LinuxBluetoothRemoteDeviceFactorySpec linuxSpec
            ? linuxSpec.ObjectPath
            : DeviceAddressToPath(spec.DeviceId, adapter);
    }

    // ==================== Native overrides ====================

    /// <inheritdoc />
    protected override void NativeRefreshIsConnected()
    {
        // Connection state is tracked via PropertiesChanged signals; no polling needed.
    }

    /// <inheritdoc />
    protected override async ValueTask NativeConnectAsync(
        Bluetooth.Abstractions.Scanning.Options.ConnectionOptions connectionOptions,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var connection = _adapter.Connection;

        // Subscribe to PropertiesChanged BEFORE calling Connect so we don't miss state changes.
        _propertiesChangedSubscription?.Dispose();
        _propertiesChangedSubscription = await BlueZObjectManager
            .WatchPropertiesChangedAsync(connection, _devicePath, OnPropertiesChanged)
            .ConfigureAwait(false);

        // Call Device1.Connect() — BlueZ blocks until connected or returns an error.
        var message = CreateConnectMessage(connection, _devicePath);
        await connection.CallMethodAsync(message).ConfigureAwait(false);

        // If Connect() returned without error, the device is connected.
        OnConnectSucceeded();
    }

    /// <inheritdoc />
    protected override async ValueTask NativeDisconnectAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var connection = _adapter.Connection;

        var message = CreateDisconnectMessage(connection, _devicePath);
        await connection.CallMethodAsync(message).ConfigureAwait(false);

        _propertiesChangedSubscription?.Dispose();
        _propertiesChangedSubscription = null;

        OnDisconnect();
    }

    /// <inheritdoc />
    protected override ValueTask NativeRequestConnectionPriorityAsync(
        ConnectionPriority priority,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        // BlueZ / Linux does not expose a connection-priority concept equivalent to Android.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeRequestMtuAsync(int requestedMtu)
    {
        // BlueZ auto-negotiates the ATT MTU; manual MTU requests are not available via D-Bus.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override void NativeReadSignalStrength()
    {
        // RSSI is reported by BlueZ via PropertiesChanged during active scanning.
        // No explicit polling is possible via Device1.
    }

    /// <inheritdoc />
    protected override async ValueTask NativeServicesExplorationAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = _adapter.Connection;

            // Wait until ServicesResolved = true.
            await WaitForServicesResolvedAsync(connection, timeout, cancellationToken).ConfigureAwait(false);

            // Retrieve all managed BlueZ objects and filter for GATT services under this device.
            var objects = await BlueZObjectManager
                .GetManagedObjectsAsync(connection, cancellationToken)
                .ConfigureAwait(false);

            var serviceObjects = objects
                .Where(o => o.HasInterface(BlueZConstants.GattService1Interface)
                            && o.Path.StartsWith(_devicePath + "/", StringComparison.Ordinal))
                .ToList();

            OnServicesExplorationSucceeded(
                serviceObjects,
                (native, existing) => existing is LinuxBluetoothRemoteService s && s.ObjectPath == native.Path,
                native =>
                {
                    var spec = new LinuxBluetoothRemoteServiceFactorySpec(native);
                    return (ServiceFactory
                            ?? throw new InvalidOperationException(
                                "ServiceFactory must be set via the spec-based constructor."))
                        .Create(this, spec);
                });
        }
        catch (Exception ex)
        {
            OnServicesExplorationFailed(ex);
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeSetPreferredPhyAsync(PhyMode txPhy, PhyMode rxPhy)
    {
        // PHY selection is not exposed via BlueZ D-Bus.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeOpenL2CapChannelAsync(int psm)
    {
        // L2CAP CoC channels are not yet supported on Linux (requires kernel socket API, not D-Bus).
        throw new NotSupportedException("L2CAP CoC channels are not currently supported on Linux.");
    }

    // ==================== Disposal ====================

    /// <inheritdoc />
    public new async ValueTask DisposeAsync()
    {
        _propertiesChangedSubscription?.Dispose();
        _propertiesChangedSubscription = null;
        await base.DisposeAsync().ConfigureAwait(false);
    }

    // ==================== Private helpers ====================

    private static MessageBuffer CreateConnectMessage(DBusConnection connection, string devicePath)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: devicePath,
            @interface: BlueZConstants.Device1Interface,
            member: BlueZConstants.MethodConnect);
        return writer.CreateMessage();
    }

    private static MessageBuffer CreateDisconnectMessage(DBusConnection connection, string devicePath)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: devicePath,
            @interface: BlueZConstants.Device1Interface,
            member: BlueZConstants.MethodDisconnect);
        return writer.CreateMessage();
    }

    /// <summary>
    ///     Handles <c>org.freedesktop.DBus.Properties.PropertiesChanged</c> signals for this device.
    /// </summary>
    private void OnPropertiesChanged(
        Exception? error,
        (string InterfaceName, IReadOnlyDictionary<string, VariantValue> Changed) args)
    {
        if (error != null || args.InterfaceName != BlueZConstants.Device1Interface)
        {
            return;
        }

        var changed = args.Changed;

        if (changed.TryGetValue(BlueZConstants.PropConnected, out var connV)
            && connV.Type == VariantValueType.Bool)
        {
            var connected = connV.GetBool();
            if (!connected && IsConnected)
            {
                // Unexpected disconnection.
                OnDisconnect();
            }
        }

        if (changed.TryGetValue(BlueZConstants.PropRSSI, out var rssiV)
            && rssiV.Type == VariantValueType.Int16)
        {
            OnSignalStrengthRead(rssiV.GetInt16());
        }
    }

    /// <summary>
    ///     Waits until BlueZ reports <c>ServicesResolved = true</c> for this device, with optional timeout.
    /// </summary>
    private async Task WaitForServicesResolvedAsync(
        DBusConnection connection,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        // Quick-check current value first.
        var props = await BlueZObjectManager
            .GetAllPropertiesAsync(connection, _devicePath, BlueZConstants.Device1Interface, cancellationToken)
            .ConfigureAwait(false);

        if (props.TryGetValue(BlueZConstants.PropServicesResolved, out var v)
            && v.Type == VariantValueType.Bool
            && v.GetBool())
        {
            return; // Already resolved.
        }

        // Wait for the PropertiesChanged signal to deliver ServicesResolved = true.
        var tcs = new TaskCompletionSource();
        IDisposable? sub = null;

        sub = await BlueZObjectManager
            .WatchPropertiesChangedAsync(
                connection,
                _devicePath,
                (ex, args) =>
                {
                    if (ex != null)
                    {
                        tcs.TrySetException(ex);
                        return;
                    }

                    if (args.InterfaceName != BlueZConstants.Device1Interface)
                    {
                        return;
                    }

                    if (args.Changed.TryGetValue(BlueZConstants.PropServicesResolved, out var sv)
                        && sv.Type == VariantValueType.Bool
                        && sv.GetBool())
                    {
                        tcs.TrySetResult();
                    }
                })
            .ConfigureAwait(false);

        try
        {
            await tcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            sub.Dispose();
        }
    }

    /// <summary>
    ///     Converts a Bluetooth address (e.g. <c>AA:BB:CC:DD:EE:FF</c>) to a BlueZ D-Bus path
    ///     using the cached adapter path.
    /// </summary>
    private static string DeviceAddressToPath(string address, LinuxBluetoothAdapter adapter)
    {
        // Synchronous best-effort path — adapter path may not be resolved yet.
        // Prefer using the LinuxBluetoothRemoteDeviceFactorySpec ObjectPath instead.
        var normalized = address.Replace(':', '_').ToUpperInvariant();
        return $"/org/bluez/hci0/dev_{normalized}";
    }

    /// <summary>
    ///     Gets the D-Bus connection adapter used by this device hierarchy.
    /// </summary>
    internal LinuxBluetoothAdapter Adapter => _adapter;

    /// <summary>
    ///     Internal D-Bus object path of this device (e.g. <c>/org/bluez/hci0/dev_AA_BB_CC_DD_EE_FF</c>).
    /// </summary>
    internal string ObjectPath => _devicePath;
}
