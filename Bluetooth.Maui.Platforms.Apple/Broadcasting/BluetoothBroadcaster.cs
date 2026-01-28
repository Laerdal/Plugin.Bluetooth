using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.PlatformSpecific.Exceptions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <summary>
/// iOS implementation of the Bluetooth broadcaster using Core Bluetooth's CBPeripheralManager.
/// </summary>
/// <remarks>
/// This implementation allows the device to act as a BLE peripheral and advertise services.
/// </remarks>
public class BluetoothBroadcaster : BaseBluetoothBroadcaster, CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate
{
    /// <summary>
    /// Gets the CbPeripheralManagerWrapper instance.
    /// </summary>
    public CbPeripheralManagerWrapper? CbPeripheralManagerWrapper { get; private set; }

    private readonly IOptions<BluetoothApplePeripheralOptions>? _options;

    // High-performance logging using LoggerMessage delegates
    private readonly static Action<ILogger, string, Exception?> _logCentralConnected =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(1, nameof(CentralConnected)),
            "Central connected: {DeviceId}");

    private readonly static Action<ILogger, string, Exception?> _logCentralDisconnected =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(2, nameof(CentralDisconnected)),
            "Central disconnected: {DeviceId}");

    /// <inheritdoc/>
    public BluetoothBroadcaster(IBluetoothAdapter adapter,
        IBluetoothBroadcastServiceFactory serviceFactory,
        IBluetoothBroadcastClientDeviceFactory deviceFactory,
        IBluetoothPermissionManager permissionManager,
        ILogger? logger = null,
        IOptions<BluetoothApplePeripheralOptions>? options = null) : base(adapter,
                                                                          serviceFactory,
                                                                          deviceFactory,
                                                                          permissionManager,
                                                                          logger)
    {
        _options = options;
    }

    /// <summary>
    /// Gets or sets the current state of the Core Bluetooth peripheral manager.
    /// </summary>
    public CBManagerState State
    {
        get => GetValue(CBManagerState.Unknown);
        private set => SetValue(value);
    }

    #region CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate

    /// <inheritdoc/>
    public void WillRestoreState(NSDictionary dict)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void DidPublishL2CapChannel(NSError? error, ushort psm)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void DidUnpublishL2CapChannel(NSError? error, ushort psm)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void StateUpdated(CBManagerState peripheralState)
    {
        State = peripheralState;
        if (Adapter is BluetoothAdapter bluetoothAdapter)
        {
            bluetoothAdapter.OnStateChanged();
        }
    }

    #endregion

    /// <inheritdoc/>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = Adapter is BluetoothAdapter { CbPeripheralManagerIsAdvertising: true };
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStartAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerWrapper);
        ArgumentNullException.ThrowIfNull(options);

        // Prepare advertisement data
        using var advertisementData = new NSMutableDictionary();

        // Add local name if provided
        if (!string.IsNullOrEmpty(options.LocalDeviceName))
        {
            advertisementData.Add(CBAdvertisement.DataLocalNameKey, new NSString(options.LocalDeviceName));
        }

        // Add service UUIDs if provided
        if (options.AdvertisedServiceUuids?.Any() == true)
        {
            var serviceUuids = options.AdvertisedServiceUuids.Select(guid => CBUUID.FromString(guid.ToString())).ToArray();
            advertisementData.Add(CBAdvertisement.DataServiceUUIDsKey, NSArray.FromObjects(serviceUuids));
        }

        // Start advertising
        CbPeripheralManagerWrapper.CbPeripheralManager.StartAdvertising(advertisementData);
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerWrapper);
        CbPeripheralManagerWrapper.CbPeripheralManager.StopAdvertising();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public void AdvertisingStarted(NSError? error)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            OnStartSucceeded();
        }
        catch (Exception ex)
        {
            OnStartFailed(ex);
        }
    }

    #region Service Management

    private readonly Dictionary<Guid, TaskCompletionSource<bool>> _serviceAddedTasks = new Dictionary<Guid, TaskCompletionSource<bool>>();

    /// <summary>
    /// Adds a new service to the peripheral manager.
    /// </summary>
    protected async Task<IBluetoothBroadcastService> NativeAddServiceAsync(Guid id,
        string name,
        bool isPrimary,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerWrapper);

        // Create the native mutable service
        var nativeService = new CBMutableService(CBUUID.FromString(id.ToString()), isPrimary);

        // Create TCS for waiting on service to be added
        var tcs = new TaskCompletionSource<bool>();
        _serviceAddedTasks[id] = tcs;

        try
        {
            // Add the native CBMutableService to the peripheral manager
            CbPeripheralManagerWrapper.CbPeripheralManager.AddService(nativeService);

            // Wait for the service to be added
            await tcs.Task.WaitAsync(timeout ?? TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);

            // Create and return the service
            var request = new BluetoothBroadcastServiceFactoryRequest
            {
                Id = id,
                Name = name,
                IsPrimary = isPrimary,
                NativeService = nativeService
            };
            return ServiceFactory.CreateBroadcastService(this, request);
        }
        finally
        {
            _serviceAddedTasks.Remove(id);
        }
    }

    /// <summary>
    /// Removes a service from the peripheral manager.
    /// </summary>
    protected Task NativeRemoveServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerWrapper);

        // Find the service
        if (Services.TryGetValue(id, out var service) && service is BluetoothBroadcastService broadcasterService)
        {
            // Remove the native CBMutableService from the peripheral manager
            CbPeripheralManagerWrapper.CbPeripheralManager.RemoveService(broadcasterService.NativeService);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void ServiceAdded(CBService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        var serviceGuid = Guid.Parse(service.UUID.ToString());
        if (_serviceAddedTasks.TryGetValue(serviceGuid, out var tcs))
        {
            tcs.TrySetResult(true);
        }
    }

    /// <inheritdoc/>
    public CbPeripheralManagerWrapper.ICbServiceDelegate GetService(CBService? characteristicService)
    {
        ArgumentNullException.ThrowIfNull(characteristicService);
        var serviceGuid = Guid.Parse(characteristicService.UUID.ToString());
        if (!Services.TryGetValue(serviceGuid, out var service))
        {
            throw new BroadcasterServiceNotFoundException(this, serviceGuid);
        }
        return service as CbPeripheralManagerWrapper.ICbServiceDelegate ?? throw new BroadcasterServiceNotFoundException(this, serviceGuid);
    }

    #endregion

    #region Client Device Management

    /// <summary>
    /// Called when a central device connects.
    /// </summary>
    /// <param name="central">The central device that connected.</param>
    public void CentralConnected(CBCentral central)
    {
        ArgumentNullException.ThrowIfNull(central);
        var request = new BluetoothBroadcastClientDeviceFactoryRequest
        {
            ClientId = central.Identifier.ToString(),
            NativeCentral = central
        };
        var device = DeviceFactory.CreateBroadcastClientDevice(this, request);
        if (Logger is not null)
        {
            _logCentralConnected(Logger, device.ClientId, null);
        }
    }

    /// <summary>
    /// Called when a central device disconnects.
    /// </summary>
    /// <param name="central">The central device that disconnected.</param>
    public void CentralDisconnected(CBCentral central)
    {
        ArgumentNullException.ThrowIfNull(central);
        var deviceId = central.Identifier.ToString();
        if (Logger is not null)
        {
            _logCentralDisconnected(Logger, deviceId, null);
        }
    }

    #endregion

    /// <summary>
    /// Initializes the peripheral manager wrapper.
    /// </summary>
    private void EnsurePeripheralManagerWrapper()
    {
        if (CbPeripheralManagerWrapper == null && Adapter is BluetoothAdapter bluetoothAdapter)
        {
            CbPeripheralManagerWrapper = bluetoothAdapter.CreateCbPeripheralManagerWrapper(this, _options);
        }
    }
}
