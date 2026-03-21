using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Permissions;
using Bluetooth.Maui.Platforms.Droid.Tools;

using ServiceNotFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.ServiceNotFoundException;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc cref="BaseBluetoothBroadcaster" />
public class AndroidBluetoothBroadcaster : BaseBluetoothBroadcaster, AdvertiseCallbackProxy.IAdvertiseCallbackProxyDelegate,
                                           BluetoothGattServerCallbackProxy.IBluetoothGattServerCallbackProxyDelegate, IAsyncDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothBroadcaster" /> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this broadcaster.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="loggerFactory">An optional logger factory for creating loggers used by this broadcaster and its components.</param>
    public AndroidBluetoothBroadcaster(IBluetoothAdapter adapter,
        ITicker ticker,
        ILoggerFactory? loggerFactory = null) : base(adapter, ticker, loggerFactory)
    {
    }

    /// <summary>
    ///     The settings that are currently in effect for the advertiser. This is set when advertising starts successfully, and is null when not advertising.
    /// </summary>
    public AdvertiseSettings? SettingsInEffect { get; private set; }

    private BluetoothManager BluetoothManager =>
        ((AndroidBluetoothAdapter) Adapter).NativeBluetoothManager;
    
    /// <inheritdoc />
    public void OnStartSuccess(AdvertiseSettings? settingsInEffect)
    {
        SettingsInEffect = settingsInEffect;
        IsRunning = true;
    }

    /// <inheritdoc />
    public void OnStartFailure(AdvertiseFailure errorCode)
    {
        SettingsInEffect = null;
        IsRunning = false;
        OnStartFailed(new AndroidNativeAdvertiseFailureException(errorCode));
    }
    
    /// <inheritdoc />
    public BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate GetDevice(BluetoothDevice? native)
    {
        ArgumentNullException.ThrowIfNull(native);
        ArgumentNullException.ThrowIfNull(native.Address);

        var device = GetClientDeviceOrDefault(native.Address) as AndroidBluetoothConnectedDevice;
        if (device == null)
        {
            var spec = new IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec(native.Address);
#pragma warning disable CA2000 // Device lifetime is managed by broadcaster client-device registry
            var createdDevice = new AndroidBluetoothConnectedDevice(this, spec);
#pragma warning restore CA2000

            createdDevice.SetNativeDevice(native);
            AddClientDevice(createdDevice);
            return createdDevice;
        }

        device.SetNativeDevice(native);
        return device;
    }

    /// <inheritdoc />
    public BluetoothGattServerCallbackProxy.IBluetoothGattServiceDelegate GetService(BluetoothGattService? native)
    {
        ArgumentNullException.ThrowIfNull(native);
        ArgumentNullException.ThrowIfNull(native.Uuid);
        var guid = native.Uuid.ToGuid();
        var service = GetServiceOrDefault(guid);
        if (service == null)
        {
            throw new ServiceNotFoundException(this, guid);
        }

        if (service is not AndroidBluetoothLocalService droidService)
        {
            throw new InvalidOperationException("Service is not Android AndroidBluetoothLocalService");
        }

        return droidService;
    }
    
    #region Start / Stop

    private AdvertiseCallbackProxy? _advertiseProxy;

    private BluetoothGattServerCallbackProxy? _gattServerProxy;

    private BluetoothLeAdvertiser? _bluetoothLeAdvertiser;

    internal bool TrySendResponse(BluetoothDevice device, int requestId, GattStatus status, int offset, byte[] value)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(value);

        return _gattServerProxy?.BluetoothGattServer.SendResponse(device, requestId, status, offset, value) ?? false;
    }

    internal BluetoothGattServer? GetGattServerOrDefault()
    {
        return _gattServerProxy?.BluetoothGattServer;
    }

    internal bool TryNotifyCharacteristicChanged(BluetoothDevice device, BluetoothGattCharacteristic characteristic, bool requiresConfirmation, byte[] value)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(characteristic);
        ArgumentNullException.ThrowIfNull(value);

        var gattServer = _gattServerProxy?.BluetoothGattServer;
        if (gattServer == null)
        {
            return false;
        }

        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            var status = gattServer.NotifyCharacteristicChanged(device, characteristic, requiresConfirmation, value);
            return (CurrentBluetoothStatusCodes) status == CurrentBluetoothStatusCodes.Success;
        }

#pragma warning disable CA1422 // Guarded by Android version check for legacy API
        return gattServer.NotifyCharacteristicChanged(device, characteristic, requiresConfirmation);
#pragma warning restore CA1422
    }
    
    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = SettingsInEffect != null;
    }

    /// <inheritdoc />
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(options);
            cancellationToken.ThrowIfCancellationRequested();

            var adapter = BluetoothManager.Adapter ?? throw new InvalidOperationException("Bluetooth adapter is not available.");

            var bluetoothLeAdvertiser = adapter.BluetoothLeAdvertiser ?? throw new PlatformNotSupportedException("Bluetooth LE advertising is not supported on this Android device.");
            _bluetoothLeAdvertiser = bluetoothLeAdvertiser;

            var advertiseProxy = _advertiseProxy ??= new AdvertiseCallbackProxy(this);

            var gattServerProxy = _gattServerProxy ??= new BluetoothGattServerCallbackProxy(this, BluetoothManager);

            foreach (var service in GetServices().OfType<AndroidBluetoothLocalService>())
            {
                if (service.NativeService != null)
                {
                    gattServerProxy.BluetoothGattServer.AddService(service.NativeService);
                }
            }

            using var advertiseSettingsBuilder = new AdvertiseSettings.Builder();
            advertiseSettingsBuilder.SetAdvertiseMode(AdvertiseMode.Balanced);
            advertiseSettingsBuilder.SetConnectable(true);
            advertiseSettingsBuilder.SetTimeout(0);
            advertiseSettingsBuilder.SetTxPowerLevel(AdvertiseTx.PowerMedium);
            using var advertiseSettings = advertiseSettingsBuilder.Build() ?? throw new InvalidOperationException("Failed to build Android advertise settings.");

            using var advertiseDataBuilder = new AdvertiseData.Builder();
            advertiseDataBuilder.SetIncludeDeviceName(options.IncludeDeviceName);
            foreach (var advertisedServiceUuid in options.AdvertisedServiceUuids ?? [])
            {
                var nativeUuid = advertisedServiceUuid.ToUuid() ?? throw new InvalidOperationException($"Failed to map advertised service UUID {advertisedServiceUuid} to Android UUID.");
                using var parcelUuid = new ParcelUuid(nativeUuid);
                advertiseDataBuilder.AddServiceUuid(parcelUuid);
            }
            using var advertiseData = advertiseDataBuilder.Build() ?? throw new InvalidOperationException("Failed to build Android advertise data.");

            bluetoothLeAdvertiser.StartAdvertising(advertiseSettings, advertiseData, advertiseProxy);
            
            return ValueTask.CompletedTask;
        }
        catch (Exception exception)
        {
            return ValueTask.FromException(exception);
        }
    }

    /// <inheritdoc />
    protected async override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_bluetoothLeAdvertiser != null && _advertiseProxy != null)
        {
            _bluetoothLeAdvertiser.StopAdvertising(_advertiseProxy);
        }

        _bluetoothLeAdvertiser = null;

        SettingsInEffect = null;

        if (_gattServerProxy != null)
        {
            _gattServerProxy.BluetoothGattServer.ClearServices();
            await CastAndDispose(_gattServerProxy).ConfigureAwait(false);
            _gattServerProxy = null;
        }

        if (_advertiseProxy != null)
        {
            await CastAndDispose(_advertiseProxy).ConfigureAwait(false);
            _advertiseProxy = null;
        }

        IsRunning = false;
        return;
        
        async static ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                resource.Dispose();
            }
        }
    }
    
    #endregion

    protected override ValueTask<IBluetoothLocalService> NativeCreateServiceAsync(Guid id,
        string? name = null,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var spec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec(id, name, isPrimary);

#pragma warning disable CA2000 // Service lifetime is owned by broadcaster service registry
        var androidService = new AndroidBluetoothLocalService(this, spec);
#pragma warning restore CA2000

        if (_gattServerProxy != null && androidService.NativeService != null)
        {
            _gattServerProxy.BluetoothGattServer.AddService(androidService.NativeService);
        }

        return new ValueTask<IBluetoothLocalService>(androidService);
    }

    #region Permission Methods

    /// <inheritdoc />
    /// <remarks>
    ///     On Android, broadcaster permissions vary by API level:
    ///     <list type="bullet">
    ///         <item>API 31+ (Android 12+): Requires BLUETOOTH_ADVERTISE permission</item>
    ///         <item>Older versions: No special permissions required for advertising</item>
    ///     </list>
    /// </remarks>
    protected async override ValueTask<bool> NativeHasBroadcasterPermissionsAsync()
    {
        try
        {
            // For API 31+ (Android 12+), need BLUETOOTH_ADVERTISE only (not CONNECT)
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                var status = await AndroidBluetoothPermissions.BluetoothAdvertisePermission.CheckStatusAsync().ConfigureAwait(false);
                return status == PermissionStatus.Granted;
            }

            // For older versions, advertising doesn't need special permissions beyond basic Bluetooth
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Android, broadcaster permissions vary by API level:
    ///     <list type="bullet">
    ///         <item>API 31+ (Android 12+): Requests BLUETOOTH_ADVERTISE permission</item>
    ///         <item>Older versions: No special permissions required for advertising</item>
    ///     </list>
    /// </remarks>
    protected async override ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken)
    {
        await AndroidBluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

        // For API 31+ (Android 12+), spec BLUETOOTH_ADVERTISE only (not CONNECT)
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            await AndroidBluetoothPermissions.BluetoothAdvertisePermission.RequestIfNeededAsync().ConfigureAwait(false);
            return;
        }

        // For older versions, no special permissions needed
    }

    #endregion

}
