using Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.Permissions;

using MultipleServicesFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.MultipleServicesFoundException;
using ServiceNotFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.ServiceNotFoundException;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothBroadcaster" />
public class AppleBluetoothBroadcaster : BaseBluetoothBroadcaster, CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate, IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothBroadcaster" /> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this broadcaster.</param>
    /// <param name="localServiceFactory">The factory for creating broadcast services.</param>
    /// <param name="connectedDeviceFactory">The factory for creating client devices.</param>
    /// <param name="cbPeripheralManagerWrapper">The Core Bluetooth peripheral manager wrapper.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    public AppleBluetoothBroadcaster(IBluetoothAdapter adapter,
        IBluetoothLocalServiceFactory localServiceFactory,
        IBluetoothConnectedDeviceFactory connectedDeviceFactory,
        CbPeripheralManagerWrapper cbPeripheralManagerWrapper,
        ITicker ticker,
        ILogger<IBluetoothBroadcaster>? logger = null) : base(adapter,
        localServiceFactory,
        connectedDeviceFactory,
        ticker,
        logger)
    {
        ArgumentNullException.ThrowIfNull(cbPeripheralManagerWrapper);
        CbPeripheralManagerWrapper = cbPeripheralManagerWrapper;
    }

    /// <summary>
    ///     Gets the Core Bluetooth peripheral manager wrapper.
    /// </summary>
    public CbPeripheralManagerWrapper CbPeripheralManagerWrapper { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Disposes the instance and releases any unmanaged resources.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is being called from the Dispose method (true) or from a finalizer (false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CbPeripheralManagerWrapper.Dispose();
        }
    }

    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = CbPeripheralManagerWrapper.CbPeripheralManager.Advertising;
    }

    #region Client Device Management

    /// <summary>
    ///     Gets an existing client device or creates and registers a new one for the specified central.
    /// </summary>
    /// <param name="central">The Core Bluetooth central device.</param>
    /// <returns>The client device corresponding to the central.</returns>
    internal IBluetoothConnectedDevice GetOrCreateClientDevice(CBCentral central)
    {
        ArgumentNullException.ThrowIfNull(central);

        var deviceId = central.Identifier.ToString();
        var device = GetClientDeviceOrDefault(deviceId);

        if (device == null)
        {
            var spec = new AppleBluetoothConnectedDeviceSpec(central);
            device = ConnectedDeviceFactory.Create(this, spec);
            AddClientDevice(device);
        }

        return device;
    }

    #endregion

    #region Start

    /// <inheritdoc />
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        CbPeripheralManagerWrapper.CbPeripheralManager.StartAdvertising(options);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void AdvertisingStarted(NSError? error)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            NativeRefreshIsRunning();
            if (!IsRunning)
            {
                throw new BroadcasterFailedToStartException(this, "Failed to start advertising for an unknown reason.");
            }

            OnStartSucceeded();
        }
        catch (Exception e)
        {
            OnStartFailed(e);
        }
    }

    #endregion

    #region Stop

    /// <inheritdoc />
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        CbPeripheralManagerWrapper.CbPeripheralManager.StopAdvertising();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void AdvertisingStopped()
    {
        try
        {
            NativeRefreshIsRunning();
            if (IsRunning)
            {
                throw new BroadcasterFailedToStopException(this, "Failed to stop advertising for an unknown reason.");
            }

            OnStopSucceeded();
        }
        catch (Exception e)
        {
            OnStopFailed(e);
        }
    }

    #endregion

    #region State

    /// <summary>
    ///     Gets or sets the current state of the Core Bluetooth central manager.
    /// </summary>
    /// <remarks>
    ///     The state indicates whether Bluetooth is powered on, off, unauthorized, unsupported, etc.
    /// </remarks>
    public CBManagerState State
    {
        get => GetValue(CBManagerState.Unknown);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public virtual void StateUpdated(CBManagerState peripheralState)
    {
        State = peripheralState;
    }

    /// <inheritdoc />
    public virtual void WillRestoreState(NSDictionary dict)
    {
        // Called when the system restores the peripheral manager state
        // Placeholder for future implementation if state restoration is needed
    }

    #endregion

    #region Status

    /// <inheritdoc />
    public void ServiceAdded(CBService service)
    {
        // Service was successfully added to the peripheral manager
        // This is called after AddService completes
        try
        {
            // You can add logic here if needed for tracking service additions
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public CbPeripheralManagerWrapper.ICbServiceDelegate GetService(CBService? characteristicService)
    {
        if (characteristicService == null)
        {
            throw new ServiceNotFoundException(this);
        }

        try
        {
            var serviceGuid = characteristicService.UUID.ToGuid();
            var match = GetServiceOrDefault(serviceGuid);
            return match as CbPeripheralManagerWrapper.ICbServiceDelegate ?? throw new ServiceNotFoundException(this, serviceGuid);
        }
        catch (InvalidOperationException e)
        {
            var serviceGuid = characteristicService.UUID.ToGuid();
            var matches = GetServices(service => service.Id == serviceGuid).ToArray();
            throw new MultipleServicesFoundException(this, matches, e);
        }
    }

    #endregion

    #region L2CAP

    /// <inheritdoc />
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        // Called when an L2CAP channel is opened
        // Placeholder for future implementation if L2CAP support is needed
    }

    /// <inheritdoc />
    public void DidPublishL2CapChannel(NSError? error, ushort psm)
    {
        // Called when an L2CAP channel PSM is published
        // Placeholder for future implementation if L2CAP support is needed
    }

    /// <inheritdoc />
    public void DidUnpublishL2CapChannel(NSError? error, ushort psm)
    {
        // Called when an L2CAP channel PSM is unpublished
        // Placeholder for future implementation if L2CAP support is needed
    }

    #endregion

    #region Permission Methods

    /// <inheritdoc />
    /// <remarks>
    ///     On iOS/macOS, broadcaster permissions require both general Bluetooth and Peripheral permissions:
    ///     <list type="bullet">
    ///         <item>iOS 13+/Mac Catalyst 10.15+: Requires both "Bluetooth Always" and "Peripheral" permissions</item>
    ///         <item>Older versions: Bluetooth permissions automatically granted</item>
    ///     </list>
    /// </remarks>
    protected async override ValueTask<bool> NativeHasBroadcasterPermissionsAsync()
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
        {
            // Check both Bluetooth Always and Peripheral permissions
            var bluetoothStatus = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<ApplePermissionForBluetoothAlways>().ConfigureAwait(false);
            var peripheralStatus = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<ApplePermissionForBluetoothPeripheral>().ConfigureAwait(false);
            return bluetoothStatus == PermissionStatus.Granted &&
                   peripheralStatus == PermissionStatus.Granted;
        }

        // On older iOS versions, Bluetooth permissions are automatically granted
        return true;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On iOS/macOS, broadcaster permissions require both general Bluetooth and Peripheral permissions:
    ///     <list type="bullet">
    ///         <item>iOS 13+/Mac Catalyst 10.15+: Requests both "Bluetooth Always" and "Peripheral" permissions</item>
    ///         <item>Older versions: No permission spec needed</item>
    ///     </list>
    /// </remarks>
    protected async override ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken)
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
        {
            // Request both Bluetooth Always and Peripheral permissions
            var bluetoothStatus = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<ApplePermissionForBluetoothAlways>().ConfigureAwait(false);
            var peripheralStatus = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<ApplePermissionForBluetoothPeripheral>().ConfigureAwait(false);

            if (bluetoothStatus != PermissionStatus.Granted || peripheralStatus != PermissionStatus.Granted)
            {
                throw new BluetoothPermissionException(
                    "Broadcaster permissions denied on iOS. User must enable Bluetooth permissions in Settings app. " +
                    "Ensure NSBluetoothAlwaysUsageDescription and NSBluetoothPeripheralUsageDescription are set in Info.plist.");
            }
        }

        // On older iOS versions, Bluetooth permissions are automatically granted
    }

    #endregion
}
