using Bluetooth.Abstractions.Broadcasting.Exceptions;
using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Abstractions.Broadcasting.Options;
using Bluetooth.Core.Infrastructure.Scheduling;
using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Tools;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcaster : BaseBluetoothBroadcaster, AdvertiseCallbackProxy.IAdvertiseCallbackProxyDelegate, BluetoothGattServerCallbackProxy.IBluetoothGattServerCallbackProxyDelegate, IAsyncDisposable
{
    private BluetoothGattServerCallbackProxy? _gattServerProxy;

    private AdvertiseCallbackProxy? _advertiseProxy;

    private BluetoothLeAdvertiser? _advertiser;

    public AdvertiseSettings? SettingsInEffect { get; private set; }

    private Android.Bluetooth.BluetoothManager BluetoothManager =>
        ((BluetoothAdapter) Adapter).NativeBluetoothManager;

    /// <inheritdoc/>
    public BluetoothBroadcaster(
        IBluetoothAdapter adapter,
        IBluetoothLocalServiceFactory localServiceFactory,
        IBluetoothConnectedDeviceFactory connectedDeviceFactory,
        IBluetoothPermissionManager permissionManager,
        ITicker ticker,
        ILogger<IBluetoothBroadcaster>? logger = null)
        : base(adapter, localServiceFactory, connectedDeviceFactory, permissionManager, ticker, logger)
    {
    }

    protected override void NativeRefreshIsRunning()
    {
        // On Android, there is no direct way to check if advertising is running.
        // We rely on the IsRunning flag being set correctly during start/stop operations.
    }

    /// <inheritdoc/>
    protected async override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothManager.Adapter);
        _advertiser ??= BluetoothManager.Adapter.BluetoothLeAdvertiser
            ?? throw new InvalidOperationException("Bluetooth LE Advertiser not available");

        _advertiseProxy ??= new AdvertiseCallbackProxy(this);

        _gattServerProxy ??= new BluetoothGattServerCallbackProxy(this, BluetoothManager);

        throw new NotImplementedException("BluetoothBroadcaster is not yet implemented on Android.");
        /*
        // Initialize GATT server proxy
        _gattServerProxy = new BluetoothGattServerCallbackProxy(this, BluetoothManager);

        // Add all services to the GATT server
        foreach (var service in Services.Values)
        {
            if (service is BluetoothBroadcastService droidService)
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

    /// <inheritdoc/>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("BluetoothBroadcaster is not yet implemented on Android.");
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

            SetValue(false, nameof(IsRunning));
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error stopping broadcaster");
            throw;
        }

        return ValueTask.CompletedTask;*/
    }

    // AdvertiseCallbackProxy.IBroadcaster implementation
    public void OnStartSuccess(AdvertiseSettings? settingsInEffect)
    {
        SettingsInEffect = settingsInEffect;
        OnStartSucceeded();
    }

    public void OnStartFailure(AdvertiseFailure errorCode)
    {
        OnStartFailed(new AndroidNativeAdvertiseFailureException(errorCode));
    }

    public BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate GetDevice(Android.Bluetooth.BluetoothDevice? nativeDevice)
    {
        ArgumentNullException.ThrowIfNull(nativeDevice);
        ArgumentNullException.ThrowIfNull(nativeDevice.Address);

        var device = GetClientDeviceOrDefault(nativeDevice.Address);
        if (device == null)
        {
            throw new ClientDeviceNotFoundException(this, nativeDevice.Address);
        }
        if (device is not BluetoothBroadcastClientDevice droidDevice)
        {
            throw new InvalidOperationException("ConnectedDevice is not Android BluetoothBroadcastClientDevice");
        }
        return droidDevice;
    }

    public BluetoothGattServerCallbackProxy.IBluetoothGattServiceDelegate GetService(Android.Bluetooth.BluetoothGattService? native)
    {
        ArgumentNullException.ThrowIfNull(native);
        ArgumentNullException.ThrowIfNull(native.Uuid);
        var guid = native.Uuid.ToGuid();
        var service = GetServiceOrDefault(guid);
        if (service == null)
        {
            throw new ServiceNotFoundException(this, guid);
        }
        if (service is not BluetoothBroadcastService droidService)
        {
            throw new InvalidOperationException("Service is not Android BluetoothBroadcastService");
        }

        return droidService;
    }

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
}
