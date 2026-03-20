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
    public AndroidBluetoothBroadcaster(IBluetoothAdapter adapter, ITicker ticker, ILoggerFactory? loggerFactory = null) : base(adapter, ticker, loggerFactory)
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
    
    #region Start / Stop

    private AdvertiseCallbackProxy? _advertiseProxy;

    private BluetoothGattServerCallbackProxy? _gattServerProxy;
    
    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        // On Android, there is no direct way to check if advertising is running.
        // We rely on the IsRunning flag being set correctly during start/stop operations.
    }

    /// <inheritdoc />
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(BluetoothManager.Adapter);

            _advertiseProxy ??= new AdvertiseCallbackProxy(this);

            _gattServerProxy ??= new BluetoothGattServerCallbackProxy(this, BluetoothManager);
            
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
        if (_gattServerProxy != null)
        {
            await CastAndDispose(_gattServerProxy).ConfigureAwait(false);
        }

        if (_advertiseProxy != null)
        {
            await CastAndDispose(_advertiseProxy).ConfigureAwait(false);
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
    
    #endregion

    protected override ValueTask<IBluetoothLocalService> NativeCreateServiceAsync(Guid id,
        string? name = null,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
