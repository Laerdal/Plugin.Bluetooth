using Android.Bluetooth;
using Android.Bluetooth.LE;

using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster,
    BluetoothGattServerCallbackProxy.IBroadcaster,
    AdvertiseCallbackProxy.IBroadcaster
{
    private readonly BluetoothManager _bluetoothManager;
    internal BluetoothGattServerCallbackProxy? GattServerProxy;
    private AdvertiseCallbackProxy? _advertiseProxy;
    private BluetoothLeAdvertiser? _advertiser;
    private readonly AutoResetEvent _advertiseStartedEvent = new(false);
    private bool _advertiseStartSuccess;

    /// <inheritdoc/>
    public BluetoothBroadcaster(IBluetoothAdapter adapter,
        IBluetoothBroadcastServiceFactory serviceFactory,
        IBluetoothBroadcastClientDeviceFactory deviceFactory,
        IBluetoothPermissionManager permissionManager,
        ILogger? logger = null) : base(adapter,
                                       serviceFactory,
                                       deviceFactory,
                                       permissionManager,
                                       logger)
    {
        var bluetoothManager = Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService) as BluetoothManager;
        _bluetoothManager = bluetoothManager ?? throw new InvalidOperationException("BluetoothManager not available");
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsRunning()
    {
        // Check if we have an active GATT server and advertiser
        IsRunning = GattServerProxy != null && _advertiser != null;
    }

    /// <inheritdoc/>
    protected async override ValueTask NativeStartAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Initialize GATT server proxy
        GattServerProxy = new BluetoothGattServerCallbackProxy(this, _bluetoothManager);

        // Add all services to the GATT server
        foreach (var service in Services.Values)
        {
            if (service is BluetoothBroadcastService droidService)
            {
                GattServerProxy.BluetoothGattServer.AddService(droidService.NativeService);
                // Wait for service to be added (OnServiceAdded callback)
                await Task.Delay(100, cancellationToken).ConfigureAwait(false);
            }
        }

        // Setup advertiser
        var bluetoothAdapter = _bluetoothManager.Adapter;
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
        var settingsBuilder = new AdvertiseSettings.Builder()
            .SetAdvertiseMode(AdvertiseMode.Balanced)
            .SetConnectable(true)
            .SetTimeout(0);

        var settings = settingsBuilder.Build() ?? throw new InvalidOperationException("Failed to build advertise settings");

        // Build advertise data
        var dataBuilder = new AdvertiseData.Builder()
            .SetIncludeDeviceName(true);

        // Add service UUIDs
        foreach (var service in Services.Values)
        {
            dataBuilder.AddServiceUuid(new Android.OS.ParcelUuid(Java.Util.UUID.FromString(service.Id.ToString())));
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

        IsRunning = true;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
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
            if (GattServerProxy != null)
            {
                GattServerProxy.Dispose();
                GattServerProxy = null;
            }

            IsRunning = false;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error stopping broadcaster");
            throw;
        }

        return ValueTask.CompletedTask;
    }

    // AdvertiseCallbackProxy.IBroadcaster implementation
    void AdvertiseCallbackProxy.IBroadcaster.OnStartSuccess(AdvertiseSettings? settingsInEffect)
    {
        _advertiseStartSuccess = true;
        _advertiseStartedEvent.Set();
        Logger?.LogInformation("Advertising started successfully");
    }

    void AdvertiseCallbackProxy.IBroadcaster.OnStartFailure(AdvertiseFailure errorCode)
    {
        _advertiseStartSuccess = false;
        _advertiseStartedEvent.Set();
        Logger?.LogError("Advertising failed to start: {ErrorCode}", errorCode);
    }

    // BluetoothGattServerCallbackProxy.IBroadcaster implementation
    BluetoothGattServerCallbackProxy.IDevice BluetoothGattServerCallbackProxy.IBroadcaster.GetDevice(Android.Bluetooth.BluetoothDevice? native)
    {
        ArgumentNullException.ThrowIfNull(native);


        // Find or create device
        var address = native.Address;
        if (address == null)
        {
            throw new InvalidOperationException("Device address is null");
        }

        var existingDevice = ConnectedDevices.Values.FirstOrDefault(d => d.Id.ToString() == address);
        if (existingDevice is BluetoothBroadcastClientDevice device)
        {
            return device;
        }

        // Create new device
        var newDevice = DeviceFactory.CreateBroadcastClientDevice(this, new IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest
        {
            ClientId = address
        });

        if (newDevice is not BluetoothBroadcastClientDevice droidDevice)
        {
            throw new InvalidOperationException("Factory did not create Android BluetoothBroadcastClientDevice");
        }

        droidDevice.NativeDevice = native;
        return droidDevice;
    }

    BluetoothGattServerCallbackProxy.IService BluetoothGattServerCallbackProxy.IBroadcaster.GetService(Android.Bluetooth.BluetoothGattService? native)
    {
        ArgumentNullException.ThrowIfNull(native);

        var uuid = Guid.Parse(native.Uuid?.ToString() ?? throw new InvalidOperationException("Service UUID is null"));

        if (!Services.TryGetValue(uuid, out var service))
        {
            throw new InvalidOperationException($"Service {uuid} not found");
        }

        if (service is not BluetoothBroadcastService droidService)
        {
            throw new InvalidOperationException("Service is not Android BluetoothBroadcastService");
        }

        return droidService;
    }
}
