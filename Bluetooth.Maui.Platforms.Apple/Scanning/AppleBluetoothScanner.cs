using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothScanner" />
public class AppleBluetoothScanner : BaseBluetoothScanner, CbCentralManagerWrapper.ICbCentralManagerDelegate, IDisposable
{
    /// <summary>
    /// Gets the CbCentralManagerWrapper instance used for managing Core Bluetooth central manager interactions.
    /// </summary>
    public CbCentralManagerWrapper CbCentralManagerWrapper { get; }

    /// <inheritdoc />
    public AppleBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothPermissionManager permissionManager,
        IBluetoothDeviceFactory deviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        IOptions<CBCentralInitOptions> options,
        IDispatchQueueProvider dispatchQueueProvider,
        ITicker ticker,
        ILogger<IBluetoothScanner>? logger = null) : base(adapter,
                                       permissionManager,
                                       deviceFactory,
                                       rssiToSignalStrengthConverter,
                                       ticker,
                                       logger)
    {
        CbCentralManagerWrapper = new CbCentralManagerWrapper(this, options, dispatchQueueProvider, ticker);
    }

    /// <summary>
    /// Disposes the instance and releases any unmanaged resources.
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
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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
        IsRunning = true;
        OnStartSucceeded();
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
        IsRunning = false;
        OnStopSucceeded();
    }

    #endregion

    #region State

    /// <summary>
    /// Gets or sets the current state of the Core Bluetooth central manager.
    /// </summary>
    /// <remarks>
    /// The state indicates whether Bluetooth is powered on, off, unauthorized, unsupported, etc.
    /// </remarks>
    public CBManagerState State
    {
        get => GetValue(CBManagerState.Unknown);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public virtual void UpdatedState(CBManagerState centralState)
    {
        State = centralState;
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
        IsRunning = CbCentralManagerWrapper.CbCentralManager.IsScanning;
        OnAdvertisementReceived(new AppleBluetoothAdvertisement(peripheral, advertisementData, rssi));
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

}
