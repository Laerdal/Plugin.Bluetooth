using Bluetooth.Core.Infrastructure.Scheduling;
using Bluetooth.Maui.Platforms.Apple.Scanning;
using Bluetooth.Maui.Platforms.Apple.Broadcasting;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.Platforms.Apple;

/// <summary>
/// iOS implementation of the Bluetooth adapter using Core Bluetooth framework.
/// </summary>
public class BluetoothAdapter : BaseBluetoothAdapter, IDisposable
{
    /// <inheritdoc/>
    public BluetoothAdapter(ITicker ticker, ILogger? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        _refreshAdapterPropertiesSubscription = ticker.Register("refresh_adapter_properties", TimeSpan.FromSeconds(1), RefreshAdapterProperties, true);
    }

    private readonly IDisposable _refreshAdapterPropertiesSubscription;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the AndroidBluetoothAdapter and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshAdapterPropertiesSubscription.Dispose();
        }
    }


    /// <summary>
    /// Refreshes cached values from native managers.
    /// </summary>
    private void RefreshAdapterProperties()
    {
        CbCentralManagerIsScanning = CbCentralManagerWrapper?.CbCentralManager.IsScanning ?? false;
        CbCentralManagerState = CbCentralManagerWrapper?.CbCentralManager.State ?? CBManagerState.Unknown;
        CbPeripheralManagerIsAdvertising = CbPeripheralManagerWrapper?.CbPeripheralManager.Advertising ?? false;
        CbPeripheralManagerState = CbPeripheralManagerWrapper?.CbPeripheralManager.State ?? CBManagerState.Unknown;
    }

    /// <summary>
    /// Called when the Bluetooth state changes.
    /// </summary>
    internal void OnStateChanged()
    {
        RefreshAdapterProperties();
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
    /// <returns>The created <see cref="CBCentralManager"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="CbCentralManagerWrapper"/> has already been created.</exception>
    public CBCentralManager GetCbCentralManager(BluetoothScanner scanner, IOptions<BluetoothAppleCentralOptions>? options)
    {
        if (CbCentralManagerWrapper != null)
        {
            throw new InvalidOperationException("CbCentralManagerWrapper has already been created.");
        }
#pragma warning disable CA2000 // Dispose objects before losing scope
        var dispatchQueue = new DispatchQueue(options?.Value.QueueLabel ?? "com.bluetooth.maui.central");
#pragma warning restore CA2000 // Dispose objects before losing scope
        var initOptions = new CBCentralInitOptions
        {
            ShowPowerAlert = options?.Value.ShowPowerAlert ?? false,
            RestoreIdentifier = options?.Value.RestoreIdentifierKey
        };
        CbCentralManagerWrapper = new CbCentralManagerWrapper(scanner, dispatchQueue, initOptions);
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
    private CbPeripheralManagerWrapper? CbPeripheralManagerWrapper { get; set; }

    /// <summary>
    /// Creates a new Core Bluetooth peripheral manager wrapper.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster instance.</param>
    /// <param name="options">The Bluetooth Apple options.</param>
    /// <returns>The created <see cref="CbPeripheralManagerWrapper"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="CbPeripheralManagerWrapper"/> has already been created.</exception>
    public CbPeripheralManagerWrapper CreateCbPeripheralManagerWrapper(BluetoothBroadcaster broadcaster, IOptions<BluetoothApplePeripheralOptions>? options)
    {
        if (CbPeripheralManagerWrapper != null)
        {
            throw new InvalidOperationException("CbPeripheralManagerWrapper has already been created.");
        }

#pragma warning disable CA2000 // Dispose objects before losing scope
        var dispatchQueue = new DispatchQueue(options?.Value.QueueLabel ?? "com.bluetooth.maui.peripheral");
#pragma warning restore CA2000 // Dispose objects before losing scope
        var initOptions = new CbPeripheralManagerOptions
        {
            ShowPowerAlert = options?.Value.ShowPowerAlert ?? false,
            RestoreIdentifierKey =  options?.Value.RestoreIdentifierKey
        };
        CbPeripheralManagerWrapper = new CbPeripheralManagerWrapper(broadcaster, dispatchQueue, initOptions);
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
