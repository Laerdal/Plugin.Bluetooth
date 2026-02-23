using Bluetooth.Maui.Platforms.Apple.Permissions;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.Threading;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothScanner" />
public partial class AppleBluetoothScanner : BaseBluetoothScanner, CbCentralManagerWrapper.ICbCentralManagerDelegate, IDisposable
{
    /// <inheritdoc />
    public AppleBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothDeviceFactory deviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        IOptions<CBCentralInitOptions> options,
        IDispatchQueueProvider dispatchQueueProvider,
        ITicker ticker,
        ILogger<IBluetoothScanner>? logger = null) : base(adapter,
        deviceFactory,
        rssiToSignalStrengthConverter,
        ticker,
        logger)
    {
        CbCentralManagerWrapper = new CbCentralManagerWrapper(this, options, dispatchQueueProvider, ticker);
    }

    /// <summary>
    ///     Gets the CbCentralManagerWrapper instance used for managing Core Bluetooth central manager interactions.
    /// </summary>
    public CbCentralManagerWrapper CbCentralManagerWrapper { get; }

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
            CbCentralManagerWrapper.Dispose();
        }
    }

    /// <inheritdoc />
    protected override IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest CreateDeviceFactoryRequestFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);
        if (advertisement is not AppleBluetoothAdvertisement appleAdvertisement)
        {
            throw new ArgumentException($"Expected advertisement of type {typeof(AppleBluetoothAdvertisement)}, but got {advertisement.GetType()}");
        }

        return new AppleBluetoothDeviceFactoryRequest(appleAdvertisement);
    }

    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = CbCentralManagerWrapper.CbCentralManager.IsScanning;
    }

    #region Start

    /// <inheritdoc />
    protected override ValueTask NativeStartAsync(ScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        CbCentralManagerWrapper.CbCentralManager.ScanForPeripherals(scanningOptions);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public virtual void ScanningStarted()
    {
        MainThreadDispatcher.BeginInvokeOnMainThread(() => {
            IsRunning = true;
            OnStartSucceeded();
        });
    }

    #endregion

    #region Stop

    /// <inheritdoc />
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        CbCentralManagerWrapper.CbCentralManager.StopScan();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public virtual void ScanningStopped()
    {
        MainThreadDispatcher.BeginInvokeOnMainThread(() => {
            IsRunning = false;
            OnStopSucceeded();
        });
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
    public virtual void UpdatedState(CBManagerState centralState)
    {
        MainThreadDispatcher.BeginInvokeOnMainThread(() => {
            State = centralState;
        });
    }

    /// <inheritdoc />
    public virtual void WillRestoreState(NSDictionary dict)
    {
        // Placeholder for future implementation if needed
    }

    #endregion

    #region Devices

    /// <inheritdoc />
    public virtual void DiscoveredPeripheral(CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi)
    {
        // Capture values before dispatching to avoid referencing native objects across threads
        var advertisement = new AppleBluetoothAdvertisement(peripheral, advertisementData, rssi);
        var isScanning = CbCentralManagerWrapper.CbCentralManager.IsScanning;

        MainThreadDispatcher.BeginInvokeOnMainThread(() => {
            IsRunning = isScanning;
            OnAdvertisementReceived(advertisement);
        });
    }

    /// <inheritdoc />
    public CbCentralManagerWrapper.ICbPeripheralDelegate GetDevice(CBPeripheral peripheral)
    {
        ArgumentNullException.ThrowIfNull(peripheral);

        try
        {
            var match = GetDeviceOrDefault(device => AreRepresentingTheSameObject(peripheral, device));
            return match as CbCentralManagerWrapper.ICbPeripheralDelegate ?? throw new DeviceNotFoundException(this, peripheral.Identifier.ToString());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetDevices(device => AreRepresentingTheSameObject(peripheral, device));
            throw new MultipleDevicesFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(CBPeripheral peripheral, IBluetoothRemoteDevice device)
    {
        return device is AppleBluetoothRemoteDevice sharedDevice
               && sharedDevice.CbPeripheralWrapper.CbPeripheral.Identifier.Equals(peripheral.Identifier)
               && sharedDevice.CbPeripheralWrapper.CbPeripheral.Handle.Handle == peripheral.Handle.Handle;
    }

    #endregion

    #region Permission Methods

    /// <inheritdoc />
    /// <remarks>
    ///     On iOS/macOS, scanner permissions are unified with general Bluetooth permissions:
    ///     <list type="bullet">
    ///         <item>iOS 13+/Mac Catalyst 10.15+: Requires "Bluetooth Always" permission</item>
    ///         <item>Older versions: Bluetooth permissions automatically granted</item>
    ///     </list>
    /// </remarks>
    protected override async ValueTask<bool> NativeHasScannerPermissionsAsync()
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
        {
            var status = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<ApplePermissionForBluetoothAlways>().ConfigureAwait(false);
            return status == PermissionStatus.Granted;
        }

        // On older iOS versions, Bluetooth permissions are automatically granted
        return true;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On iOS/macOS, scanner permissions are unified with general Bluetooth permissions:
    ///     <list type="bullet">
    ///         <item>iOS 13+/Mac Catalyst 10.15+: Requests "Bluetooth Always" permission</item>
    ///         <item>Older versions: No permission request needed</item>
    ///     </list>
    ///     The <paramref name="requireBackgroundLocation"/> parameter is ignored on iOS (background permissions handled by Info.plist).
    /// </remarks>
    protected override async ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken)
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
        {
            var status = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<ApplePermissionForBluetoothAlways>().ConfigureAwait(false);
            if (status != PermissionStatus.Granted)
            {
                throw new BluetoothPermissionException(
                    "Bluetooth permission denied on iOS. User must enable Bluetooth permissions in Settings app. " +
                    "Ensure NSBluetoothAlwaysUsageDescription is set in Info.plist.");
            }
        }

        // On older iOS versions, Bluetooth permissions are automatically granted
    }

    #endregion
}