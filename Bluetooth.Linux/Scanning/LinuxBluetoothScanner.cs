using Bluetooth.Linux.Scanning.Factories;

namespace Bluetooth.Linux.Scanning;

/// <summary>
///     Linux implementation of the Bluetooth scanner using BlueZ via D-Bus.
/// </summary>
/// <remarks>
///     Uses <see cref="BlueZManager"/> to obtain the first available HCI adapter and subscribes
///     to its <c>DeviceFound</c> event to surface BLE advertisement data.
///     BlueZ does not expose raw advertisement PDUs — device properties (Name, RSSI, UUIDs, ManufacturerData)
///     are read from the D-Bus Device1 interface when each device is found.
/// </remarks>
public class LinuxBluetoothScanner : BaseBluetoothScanner
{
    private Adapter? _bluezAdapter;
    private bool _isDiscovering;

    // Keeps native Device objects alive between discovery and NativeCreateDeviceFromAdvertisement
    private readonly Dictionary<string, Device> _nativeDeviceCache = new(StringComparer.OrdinalIgnoreCase);

    private readonly IBluetoothRemoteDeviceFactory _deviceFactory;

    /// <inheritdoc />
    public LinuxBluetoothScanner(
        IBluetoothAdapter adapter,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ITicker ticker,
        IBluetoothRemoteDeviceFactory deviceFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null)
        : base(adapter, rssiToSignalStrengthConverter, ticker, nameProvider, loggerFactory)
    {
        _deviceFactory = deviceFactory ?? throw new ArgumentNullException(nameof(deviceFactory));
    }

    #region BlueZ adapter initialisation

    private async Task<Adapter> GetBluezAdapterAsync(CancellationToken cancellationToken = default)
    {
        if (_bluezAdapter != null)
            return _bluezAdapter;

        var adapters = await BlueZManager.GetAdaptersAsync().ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        if (adapters.Count == 0)
            throw new InvalidOperationException("No Bluetooth adapters found. Ensure BlueZ is running and a Bluetooth adapter is present.");

        _bluezAdapter = adapters[0];
        _bluezAdapter.DeviceFound += OnDeviceFoundAsync;
        return _bluezAdapter;
    }

    #endregion

    #region Device-found event

    private Task OnDeviceFoundAsync(Adapter sender, DeviceFoundEventArgs e)
    {
        // Fire-and-forget: fetch device properties and surface as advertisement
        _ = ProcessDeviceFoundAsync(e.Device);
        return Task.CompletedTask;
    }

    private async Task ProcessDeviceFoundAsync(Device device)
    {
        try
        {
            var props = await device.GetAllAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(props.Address))
                return;

            // Cache native device by MAC address so NativeCreateDeviceFromAdvertisement can retrieve it
            _nativeDeviceCache[props.Address] = device;

            var advertisement = BuildAdvertisement(props, device);
            OnAdvertisementReceived(advertisement);
        }
        catch (Exception ex)
        {
            Logger?.LogDebug(ex, "Failed to read properties for discovered device");
        }
    }

    private LinuxBluetoothAdvertisement BuildAdvertisement(Device1Properties props, Device device)
    {
        var serviceGuids = (props.UUIDs ?? [])
            .Select(u =>
            {
                var normalized = BlueZManager.NormalizeUUID(u);
                return Guid.TryParse(normalized, out var g) ? g : Guid.Empty;
            })
            .Where(g => g != Guid.Empty)
            .ToArray();

        var manufacturerData = ExtractManufacturerData(props.ManufacturerData);

        return new LinuxBluetoothAdvertisement(
            address: props.Address ?? string.Empty,
            name: props.Name,
            rssi: props.RSSI,
            txPower: props.TxPower,
            serviceUuids: serviceGuids,
            manufacturerData: manufacturerData,
            isConnectable: true); // BlueZ doesn't expose a Connectable flag at this level
    }

    private static ReadOnlyMemory<byte> ExtractManufacturerData(IDictionary<ushort, object>? raw)
    {
        if (raw == null || raw.Count == 0)
            return ReadOnlyMemory<byte>.Empty;

        // BlueZ stores ManufacturerData as { companyId => byte[] }
        // Encode as: [id_lo, id_hi, ...data_bytes] matching standard AD structure
        var first = raw.First();
        ushort companyId = first.Key;
        var payload = first.Value is byte[] bytes ? bytes : [];

        var result = new byte[2 + payload.Length];
        result[0] = (byte)(companyId & 0xFF);
        result[1] = (byte)(companyId >> 8);
        Array.Copy(payload, 0, result, 2, payload.Length);
        return result;
    }

    #endregion

    #region BaseBluetoothScanner abstract implementations

    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = _isDiscovering;
    }

    /// <inheritdoc />
    protected override async ValueTask NativeStartAsync(ScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var adapter = await GetBluezAdapterAsync(cancellationToken).ConfigureAwait(false);
        await adapter.StartDiscoveryAsync().ConfigureAwait(false);
        _isDiscovering = true;
    }

    /// <inheritdoc />
    protected override async ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (_bluezAdapter == null)
            return;

        await _bluezAdapter.StopDiscoveryAsync().ConfigureAwait(false);
        _isDiscovering = false;
    }

    /// <inheritdoc />
    protected override ValueTask<bool> NativeHasScannerPermissionsAsync()
    {
        // On Linux, Bluetooth access is controlled by system D-Bus policy and the 'bluetooth' group.
        // There is no runtime permission request API — if D-Bus access is denied, operations will throw.
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc />
    protected override ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken)
    {
        // No runtime permission API on Linux — managed via D-Bus policy and system groups.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override IBluetoothRemoteDevice NativeCreateDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        if (_nativeDeviceCache.TryGetValue(advertisement.BluetoothAddress, out var nativeDevice))
        {
            var linuxSpec = new LinuxBluetoothRemoteDeviceFactorySpec(nativeDevice, advertisement);
            return _deviceFactory.Create(this, linuxSpec);
        }

        // Fallback: create without native device — connect will fail with a clear message
        var spec = new IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec(advertisement);
        return _deviceFactory.Create(this, spec);
    }

    #endregion

    #region Disposal

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        if (_bluezAdapter != null)
        {
            _bluezAdapter.DeviceFound -= OnDeviceFoundAsync;
            _bluezAdapter.Dispose();
            _bluezAdapter = null;
        }

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    #endregion
}
