using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Permissions;
using Bluetooth.Maui.Platforms.Droid.Tools;

using ServiceNotFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.ServiceNotFoundException;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc cref="BaseBluetoothBroadcaster" />
public class AndroidBluetoothBroadcaster : BaseBluetoothBroadcaster, AdvertiseCallbackProxy.IAdvertiseCallbackProxyDelegate, BluetoothGattServerCallbackProxy.IBluetoothGattServerCallbackProxyDelegate, IAsyncDisposable
{
    private AdvertiseCallbackProxy? _advertiseProxy;

    private BluetoothLeAdvertiser? _advertiser;
    private BluetoothGattServerCallbackProxy? _gattServerProxy;

    /// <inheritdoc />
    public AndroidBluetoothBroadcaster(
        IBluetoothAdapter adapter,
        IBluetoothLocalServiceFactory localServiceFactory,
        IBluetoothConnectedDeviceFactory connectedDeviceFactory,
        ITicker ticker,
        ILogger<IBluetoothBroadcaster>? logger = null)
        : base(adapter, localServiceFactory, connectedDeviceFactory, ticker, logger)
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
        OnStartSucceeded();
    }


    /// <inheritdoc />
    public void OnStartFailure(AdvertiseFailure errorCode)
    {
        OnStartFailed(new AndroidNativeAdvertiseFailureException(errorCode));
    }

    /// <inheritdoc />
    public new async ValueTask DisposeAsync()
    {
        if (_gattServerProxy != null)
        {
            await CastAndDispose(_gattServerProxy).ConfigureAwait(false);
        }

        if (_advertiseProxy != null)
        {
            await CastAndDispose(_advertiseProxy).ConfigureAwait(false);
        }

        if (_advertiser != null)
        {
            await CastAndDispose(_advertiser).ConfigureAwait(false);
        }

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

    /// <inheritdoc />
    public BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate GetDevice(BluetoothDevice? native)
    {
        ArgumentNullException.ThrowIfNull(native);
        ArgumentNullException.ThrowIfNull(native.Address);

        var device = GetClientDeviceOrDefault(native.Address);
        if (device == null)
        {
            throw new ClientDeviceNotFoundException(this, native.Address);
        }

        if (device is not AndroidBluetoothConnectedDevice droidDevice)
        {
            throw new InvalidOperationException("ConnectedDevice is not Android AndroidBluetoothConnectedDevice");
        }

        return droidDevice;
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

    protected override void NativeRefreshIsRunning()
    {
        // On Android, there is no direct way to check if advertising is running.
        // We rely on the IsRunning flag being set correctly during start/stop operations.
    }

    /// <inheritdoc />
    protected async override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothManager.Adapter);
        _advertiser ??= BluetoothManager.Adapter.BluetoothLeAdvertiser
                        ?? throw new InvalidOperationException("Bluetooth LE Advertiser not available");

        _advertiseProxy ??= new AdvertiseCallbackProxy(this);

        _gattServerProxy ??= new BluetoothGattServerCallbackProxy(this, BluetoothManager);

        throw new NotImplementedException("AndroidBluetoothBroadcaster is not yet implemented on Android.");
        /*
        // Initialize GATT server proxy
        _gattServerProxy = new BluetoothGattServerCallbackProxy(this, BluetoothManager);

        // Add all services to the GATT server
        foreach (var service in Services.Values)
        {
            if (service is AndroidBluetoothLocalService droidService)
            {
                _gattServerProxy.BluetoothGattServer.AddService(droidService.NativeService);

                // Wait for service to be added (OnServiceAdded callback)
                await Task.Delay(100, cancellationToken).ConfigureAwait(false);
            }
        }

        // Setup advertiser
        var bluetoothAdapter = BluetoothManager.Adapter;
        if (bluetoothAdapter == null)
        {
            throw new InvalidOperationException("Bluetooth adapter not available");
        }

        _advertiser = bluetoothAdapter.BluetoothLeAdvertiser;
        if (_advertiser == null)
        {
            throw new InvalidOperationException("Bluetooth LE Advertiser not available");
        }

        _advertiseProxy = new AdvertiseCallbackProxy(this);

        // Build advertise settings
        using var settingsBuilder = new AdvertiseSettings.Builder();
        settingsBuilder.SetAdvertiseMode(AdvertiseMode.Balanced);
        settingsBuilder.SetConnectable(true);
        settingsBuilder.SetTimeout(0); // No timeout
        var settings = settingsBuilder.Build() ?? throw new InvalidOperationException("Failed to build advertise settings");

        // Build advertise data
        using var dataBuilder = new AdvertiseData.Builder();
        dataBuilder.SetIncludeDeviceName(true);

        // Add service UUIDs
        foreach (var service in Services.Values)
        {
            using var uuid = new Android.OS.ParcelUuid(Java.Util.UUID.FromString(service.Id.ToString()));
            dataBuilder.AddServiceUuid(uuid);
        }

        var data = dataBuilder.Build() ?? throw new InvalidOperationException("Failed to build advertise data");

        // Start advertising
        _advertiseStartedEvent.Reset();
        _advertiser.StartAdvertising(settings, data, _advertiseProxy);

        // Wait for advertising to start
        var waitTimeout = timeout ?? TimeSpan.FromSeconds(10);
        if (!_advertiseStartedEvent.WaitOne(waitTimeout))
        {
            throw new TimeoutException("Timeout waiting for advertising to start");
        }

        if (!_advertiseStartSuccess)
        {
            throw new InvalidOperationException("Failed to start advertising");
        }

        SetValue(true, nameof(IsRunning));*/
    }

    /// <inheritdoc />
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("AndroidBluetoothBroadcaster is not yet implemented on Android.");
        /*
        try
        {
            // Stop advertising
            if (_advertiser != null && _advertiseProxy != null)
            {
                _advertiser.StopAdvertising(_advertiseProxy);
                _advertiser = null;
                _advertiseProxy = null;
            }

            // Close GATT server
            if (_gattServerProxy != null)
            {
                _gattServerProxy.Dispose();
                _gattServerProxy = null;
            }

            IsRunning = false;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error stopping broadcaster");
            throw;
        }

        return ValueTask.CompletedTask;*/
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
    protected override async ValueTask<bool> NativeHasBroadcasterPermissionsAsync()
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
    protected override async ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken)
    {
        await AndroidBluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

        // For API 31+ (Android 12+), request BLUETOOTH_ADVERTISE only (not CONNECT)
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            await AndroidBluetoothPermissions.BluetoothAdvertisePermission.RequestIfNeededAsync().ConfigureAwait(false);
            return;
        }

        // For older versions, no special permissions needed
    }

    #endregion
}
