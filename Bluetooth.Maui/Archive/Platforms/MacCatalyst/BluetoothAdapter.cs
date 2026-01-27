using Bluetooth.Maui.PlatformSpecific;

using Microsoft.Extensions.Options;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of the Bluetooth adapter using Core Bluetooth framework.
/// </summary>
public class BluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc/>
    protected override void NativeRefreshValues()
    {
        CbCentralManagerIsScanning = CbCentralManagerWrapper?.CbCentralManager.IsScanning ?? false;
        CbCentralManagerState = CbCentralManagerWrapper?.CbCentralManager.State ?? CBManagerState.Unknown;
        CbPeripheralManagerIsAdvertising = CbPeripheralManagerWrapper?.CbPeripheralManager.Advertising ?? false;
        CbPeripheralManagerState = CbPeripheralManagerWrapper?.CbPeripheralManager.State ?? CBManagerState.Unknown;
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsBluetoothOn()
    {
        IsBluetoothOn = CbCentralManagerState == CBManagerState.PoweredOn || CbPeripheralManagerState == CBManagerState.PoweredOn;
    }

    internal void OnStateChanged()
    {
        NativeRefreshIsBluetoothOn();
    }

    #region Scanner

    /// <summary>
    /// Gets the Core Bluetooth central manager wrapper.
    /// </summary>
    private CbCentralManagerWrapper? CbCentralManagerWrapper { get; set; }

    /// <summary>
    /// Creates a new Core Bluetooth central manager wrapper.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner instance.</param>
    /// <param name="options">The Bluetooth Apple options.</param>
    /// <returns>The created <see cref="CbCentralManagerWrapper"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="CbCentralManagerWrapper"/> has already been created.</exception>
    public CBCentralManager GetCbCentralManagerWrapper(BluetoothScanner scanner, IOptions<BluetoothAppleOptions>? options)
    {
        if (CbCentralManagerWrapper != null)
        {
            throw new InvalidOperationException("CbCentralManagerWrapper has already been created.");
        }

        CbCentralManagerWrapper = new CbCentralManagerWrapper(scanner, options);
        return CbCentralManagerWrapper.CbCentralManager;
    }

    /// <summary>
    /// Gets a value indicating whether the Core Bluetooth central manager is currently scanning for peripherals.
    /// </summary>
    public bool CbCentralManagerIsScanning
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the current state of the Core Bluetooth central manager.
    /// </summary>
    public CBManagerState CbCentralManagerState
    {
        get => GetValue(CBManagerState.Unknown);
        private set => SetValue(value);
    }

    #endregion

    #region Broadcaster

    /// <summary>
    /// Gets the Core Bluetooth peripheral manager wrapper.
    /// </summary>
    public CbPeripheralManagerWrapper? CbPeripheralManagerWrapper { get; private set; }

    /// <summary>
    /// Creates a new Core Bluetooth peripheral manager wrapper.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster instance.</param>
    /// <param name="options">The Bluetooth Apple options.</param>
    /// <returns>The created <see cref="CbPeripheralManagerWrapper"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="CbPeripheralManagerWrapper"/> has already been created.</exception>
    public CbPeripheralManagerWrapper CreateCbPeripheralManagerWrapper(BluetoothBroadcaster broadcaster, IOptions<BluetoothAppleOptions>? options)
    {
        if (CbPeripheralManagerWrapper != null)
        {
            throw new InvalidOperationException("CbPeripheralManagerWrapper has already been created.");
        }

        CbPeripheralManagerWrapper = new CbPeripheralManagerWrapper(broadcaster, options);
        return CbPeripheralManagerWrapper;
    }

    /// <summary>
    /// Gets a value indicating whether the Core Bluetooth peripheral manager is currently advertising.
    /// </summary>
    public bool CbPeripheralManagerIsAdvertising
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the current state of the Core Bluetooth peripheral manager.
    /// </summary>
    public CBManagerState CbPeripheralManagerState
    {
        get => GetValue(CBManagerState.Unknown);
        private set => SetValue(value);
    }

    #endregion

}
