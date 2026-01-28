using Bluetooth.Maui.Platforms.Apple.PlatformSpecific;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
/// iOS implementation of the Bluetooth scanner using Core Bluetooth framework.
/// </summary>
/// <remarks>
/// This implementation uses <see cref="CBCentralManager"/> to scan for BLE peripherals.
/// </remarks>
public partial class BluetoothScanner : BaseBluetoothScanner, CbCentralManagerWrapper.ICbCentralManagerProxyDelegate
{
    /// <summary>
    /// Gets the CbCentralManager instance.
    /// </summary>
    public CBCentralManager? CbCentralManager { get; private set; }
    private readonly IOptions<BluetoothAppleCentralOptions>? _options;

    /// <inheritdoc/>
    public BluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothPermissionManager permissionManager,
        IBluetoothDeviceFactory deviceFactory,
        IBluetoothCharacteristicAccessServicesRepository knownServicesAndCharacteristicsRepository,
        ILogger? logger = null,
        IOptions<BluetoothAppleCentralOptions>? options = null) : base(adapter,
                                                                       permissionManager,
                                                                       deviceFactory,
                                                                       knownServicesAndCharacteristicsRepository,
                                                                       logger)
    {
        _options = options;
    }

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

    #region CbCentralManagerWrapper

    /// <summary>
    /// Called when the central manager is about to restore its state.
    /// </summary>
    /// <param name="dict">A dictionary containing state restoration information.</param>
    /// <remarks>
    /// This is called during state restoration when the app is relaunched in the background.
    /// </remarks>
    public void WillRestoreState(NSDictionary dict)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void UpdatedState(CBManagerState centralState)
    {
        State = centralState;
        if (Adapter is BluetoothAdapter adapter)
        {
            adapter.OnStateChanged();
        }
    }

    /// <summary>
    /// Called when the central manager discovers a peripheral.
    /// </summary>
    /// <param name="peripheral">The discovered peripheral.</param>
    /// <param name="advertisementData">The advertisement data from the peripheral.</param>
    /// <param name="rssi">The received signal strength indicator (RSSI) in dBm.</param>
    /// <remarks>
    /// This method creates a <see cref="BluetoothAdvertisement"/> from the discovery data and processes it.
    /// </remarks>
    public void DiscoveredPeripheral(CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi)
    {
        NativeRefreshIsRunning();
        OnStartSucceeded(); // If we received a peripheral, we can assume that the start scan was successful
        OnAdvertisementReceived(new BluetoothAdvertisement(peripheral, advertisementData, rssi));
    }

    #endregion

    /// <inheritdoc/>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = Adapter is BluetoothAdapter { CbCentralManagerIsScanning: true };
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Starts scanning for peripherals using the Core Bluetooth central manager.
    /// The method determines which overload to call based on available service UUIDs and options.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="CbCentralManager"/> is <c>null</c>.</exception>
    protected override ValueTask NativeStartAsync(IBluetoothScannerStartScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        CbCentralManager ??= ((BluetoothAdapter)Adapter).GetCbCentralManager(this, _options);
        CbCentralManager.ScanForPeripherals(scanningOptions);
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops scanning for peripherals using the Core Bluetooth central manager.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="CbCentralManager"/> is <c>null</c>.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbCentralManager);
        CbCentralManager.StopScan();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

    #region Device Management

    /// <summary>
    /// Gets the device proxy for a given Core Bluetooth peripheral.
    /// </summary>
    /// <param name="peripheral">The peripheral to find.</param>
    /// <returns>The device proxy for the peripheral.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="peripheral"/> is <c>null</c>.</exception>
    /// <exception cref="DeviceNotFoundException">Thrown when the device is not found in the device list.</exception>
    /// <exception cref="MultipleDevicesFoundException">Thrown when multiple devices match the peripheral identifier.</exception>
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

    private static bool AreRepresentingTheSameObject(CBPeripheral peripheral, IBluetoothDevice device)
    {
        return device is BluetoothDevice sharedDevice
            && sharedDevice.CbPeripheralWrapper.CbPeripheral.Identifier.ToString() == peripheral.Identifier.ToString()
            && sharedDevice.CbPeripheralWrapper.CbPeripheral.Handle.Handle == peripheral.Handle.Handle;
    }

    #endregion
}
