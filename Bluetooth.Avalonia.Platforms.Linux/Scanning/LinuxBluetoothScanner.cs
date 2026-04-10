using Bluetooth.Avalonia.Platforms.Linux.Permissions;
using Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <summary>
///     Linux BLE scanner backed by BlueZ via D-Bus.
/// </summary>
/// <remarks>
///     Scanning is performed by calling <c>org.bluez.Adapter1.StartDiscovery</c> and subscribing
///     to <c>org.freedesktop.DBus.ObjectManager.InterfacesAdded</c> / <c>InterfacesRemoved</c>
///     signals on the system bus.
/// </remarks>
public class LinuxBluetoothScanner : BaseBluetoothScanner
{
    private readonly LinuxBluetoothAdapter _linuxAdapter;
    private readonly IBluetoothRemoteDeviceFactory _deviceFactory;
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _addressToObjectPath = new();

    private IDisposable? _interfacesAddedSubscription;
    private IDisposable? _interfacesRemovedSubscription;

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
        if (adapter is not LinuxBluetoothAdapter linuxAdapter)
        {
            throw new ArgumentException(
                $"Adapter must be a {nameof(LinuxBluetoothAdapter)} for the Linux platform.", nameof(adapter));
        }

        _linuxAdapter = linuxAdapter;
        _deviceFactory = deviceFactory;
    }

    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        // Discovery state is tracked manually via StartDiscovery / StopDiscovery;
        // no additional refresh needed on Linux.
    }

    /// <inheritdoc />
    protected override async ValueTask NativeStartAsync(
        ScanningOptions scanningOptions,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var adapterPath = await _linuxAdapter.GetAdapterPathAsync(cancellationToken).ConfigureAwait(false);
        var connection = _linuxAdapter.Connection;

        // Subscribe to InterfacesAdded to receive new device advertisements.
        _interfacesAddedSubscription = await BlueZObjectManager
            .WatchInterfacesAddedAsync(connection, OnInterfacesAdded)
            .ConfigureAwait(false);

        // Subscribe to InterfacesRemoved so we can mark devices stale.
        _interfacesRemovedSubscription = await BlueZObjectManager
            .WatchInterfacesRemovedAsync(connection, OnInterfacesRemoved)
            .ConfigureAwait(false);

        // Replay already-known devices (devices BlueZ cached before this scan started).
        await ReplayExistingDevicesAsync(connection, cancellationToken).ConfigureAwait(false);

        // Start active discovery on the adapter.
        var message = CreateStartDiscoveryMessage(connection, adapterPath);
        await connection.CallMethodAsync(message).ConfigureAwait(false);

        IsRunning = true;
    }

    /// <inheritdoc />
    protected override async ValueTask NativeStopAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        _interfacesAddedSubscription?.Dispose();
        _interfacesAddedSubscription = null;

        _interfacesRemovedSubscription?.Dispose();
        _interfacesRemovedSubscription = null;

        var adapterPath = await _linuxAdapter.GetAdapterPathAsync(cancellationToken).ConfigureAwait(false);
        var connection = _linuxAdapter.Connection;

        var message = CreateStopDiscoveryMessage(connection, adapterPath);

        try
        {
            await connection.CallMethodAsync(message).ConfigureAwait(false);
        }
        catch
        {
            // StopDiscovery may fail if discovery was already stopped; ignore.
        }

        IsRunning = false;
    }

    /// <inheritdoc />
    protected override async ValueTask<bool> NativeHasScannerPermissionsAsync()
    {
        return await LinuxBluetoothPermissions.HasBluetoothPermissionsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override ValueTask NativeRequestScannerPermissionsAsync(
        bool requireBackgroundLocation,
        CancellationToken cancellationToken)
    {
        // On Linux, permissions are granted at the OS level (bluetooth group / capabilities).
        // There is no runtime permission dialog; we cannot request them programmatically.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override IBluetoothRemoteDevice NativeCreateDeviceFromAdvertisement(
        IBluetoothAdvertisement advertisement)
    {
        // Try to resolve the D-Bus object path from the cached address-to-path map.
        if (_addressToObjectPath.TryGetValue(advertisement.BluetoothAddress, out var objectPath))
        {
            var spec = new LinuxBluetoothRemoteDeviceFactorySpec(objectPath, advertisement);
            return _deviceFactory.Create(this, spec);
        }

        // Fallback: create device with address only (path guessed from address).
        var fallbackSpec = new IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec(advertisement);
        return _deviceFactory.Create(this, fallbackSpec);
    }

    // ==================== Helpers ====================

    private static MessageBuffer CreateStartDiscoveryMessage(DBusConnection connection, string adapterPath)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: adapterPath,
            @interface: BlueZConstants.Adapter1Interface,
            member: BlueZConstants.MethodStartDiscovery);
        return writer.CreateMessage();
    }

    private static MessageBuffer CreateStopDiscoveryMessage(DBusConnection connection, string adapterPath)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: adapterPath,
            @interface: BlueZConstants.Adapter1Interface,
            member: BlueZConstants.MethodStopDiscovery);
        return writer.CreateMessage();
    }

    /// <summary>
    ///     Processes cached BlueZ device objects that existed before scanning started.
    /// </summary>
    private async Task ReplayExistingDevicesAsync(DBusConnection connection, CancellationToken ct)
    {
        var objects = await BlueZObjectManager.GetManagedObjectsAsync(connection, ct).ConfigureAwait(false);
        foreach (var obj in objects)
        {
            if (obj.HasInterface(BlueZConstants.Device1Interface))
            {
                ProcessDeviceObject(obj);
            }
        }
    }

    /// <summary>
    ///     Called when BlueZ adds new D-Bus objects (e.g. a newly discovered device).
    /// </summary>
    private void OnInterfacesAdded(
        Exception? error,
        (string Path, IReadOnlyDictionary<string, IReadOnlyDictionary<string, VariantValue>> Interfaces) args)
    {
        if (error != null)
        {
            return;
        }

        var (path, interfaces) = args;
        if (!interfaces.ContainsKey(BlueZConstants.Device1Interface))
        {
            return;
        }

        var obj = new BlueZObjectInfo(path, interfaces);
        ProcessDeviceObject(obj);
    }

    /// <summary>
    ///     Called when BlueZ removes D-Bus objects (e.g. device removed).
    /// </summary>
    private void OnInterfacesRemoved(Exception? error, (string Path, string[] Interfaces) args)
    {
        // Device removal is handled by the base-class disappearance timer.
    }

    /// <summary>
    ///     Converts a <see cref="BlueZObjectInfo" /> into an advertisement and passes it to the base class.
    /// </summary>
    private void ProcessDeviceObject(BlueZObjectInfo obj)
    {
        // Cache address → D-Bus path so NativeCreateDeviceFromAdvertisement can look it up.
        var address = obj.GetStringProp(BlueZConstants.Device1Interface, BlueZConstants.PropAddress);
        if (address != null)
        {
            _addressToObjectPath[address] = obj.Path;
        }

        var advertisement = BuildAdvertisement(obj);
        OnAdvertisementReceived(advertisement);
    }

    /// <summary>
    ///     Builds a <see cref="BaseBluetoothAdvertisement" /> from a BlueZ Device1 object.
    /// </summary>
    private static BaseBluetoothAdvertisement BuildAdvertisement(BlueZObjectInfo obj)
    {
        var deviceProps = obj.Interfaces.TryGetValue(BlueZConstants.Device1Interface, out var p) ? p
            : new Dictionary<string, VariantValue>();

        var address = deviceProps.TryGetValue(BlueZConstants.PropAddress, out var addrV)
                      && addrV.Type == VariantValueType.String
            ? addrV.GetString()
            : string.Empty;

        // Prefer Alias (user-editable) over Name (from device).
        var name = (deviceProps.TryGetValue(BlueZConstants.PropAlias, out var aliasV)
                    && aliasV.Type == VariantValueType.String
            ? aliasV.GetString()
            : null)
                   ?? (deviceProps.TryGetValue(BlueZConstants.PropName, out var nameV)
                       && nameV.Type == VariantValueType.String
                       ? nameV.GetString()
                       : null);

        var rssi = deviceProps.TryGetValue(BlueZConstants.PropRSSI, out var rssiV)
                   && rssiV.Type == VariantValueType.Int16
            ? rssiV.GetInt16()
            : (short) 0;

        var txPower = deviceProps.TryGetValue(BlueZConstants.PropTxPower, out var txV)
                      && txV.Type == VariantValueType.Int16
            ? txV.GetInt16()
            : (short) 0;

        var uuids = deviceProps.TryGetValue(BlueZConstants.PropUUIDs, out var uuidsV)
                    && uuidsV.Type == VariantValueType.Array
            ? uuidsV.GetArray<string>()
                    .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
                    .Where(g => g != Guid.Empty)
            : Enumerable.Empty<Guid>();

        var manufacturerData = BuildManufacturerData(deviceProps);

        return new BaseBluetoothAdvertisement(
            deviceName: name,
            servicesGuids: uuids,
            isConnectable: true,
            rawSignalStrengthInDBm: rssi,
            transmitPowerLevelInDBm: txPower,
            bluetoothAddress: address,
            manufacturerData: manufacturerData);
    }

    /// <summary>
    ///     Extracts manufacturer data bytes from the <c>ManufacturerData</c> property.
    ///     BlueZ represents it as <c>a{qv}</c> (company_id → byte[]).
    ///     We serialize it as a 2-byte little-endian company ID followed by the payload,
    ///     matching the convention used by Android and Windows implementations.
    /// </summary>
    private static ReadOnlyMemory<byte> BuildManufacturerData(
        IReadOnlyDictionary<string, VariantValue> deviceProps)
    {
        if (!deviceProps.TryGetValue(BlueZConstants.PropManufacturerData, out var mdV))
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        if (mdV.Type != VariantValueType.Array)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        var dict = mdV.GetDictionary<ushort, VariantValue>();
        if (dict.Count == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        // Use the first entry (convention: there is normally only one).
        var (companyId, payloadVariant) = dict.First();

        byte[] payload = payloadVariant.Type == VariantValueType.Array
            ? payloadVariant.GetArray<byte>()
            : [];

        // Build: [companyId_lo, companyId_hi, ...payload]
        var result = new byte[2 + payload.Length];
        result[0] = (byte) (companyId & 0xFF);
        result[1] = (byte) (companyId >> 8);
        payload.CopyTo(result, 2);

        return result;
    }
}
