namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

/// <summary>
///     Proxy class for CoreBluetooth central manager delegate callbacks.
///     https://developer.apple.com/documentation/corebluetooth/cbcentralmanagerdelegate
/// </summary>
public partial class CbCentralManagerWrapper : CBCentralManagerDelegate
{
    private readonly ICbCentralManagerDelegate _cbCentralManagerDelegate;

    private readonly IDispatchQueueProvider _dispatchQueueProvider;

    private readonly CBCentralInitOptions _options;

    private readonly ITicker _ticker;
    private readonly object _lock = new object();
    private CBCentralManager? _cbCentralManager;

    private IDisposable? _refreshSubscription;

    /// <summary>
    ///     Initializes a new instance of the CbCentralManagerWrapper class.
    /// </summary>
    /// <param name="cbCentralManagerDelegate"></param>
    /// <param name="options">The initialization options for the central manager.</param>
    /// <param name="dispatchQueueProvider">The provider for dispatch queues.</param>
    /// <param name="ticker">The ticker for scheduling tasks.</param>
    public CbCentralManagerWrapper(ICbCentralManagerDelegate cbCentralManagerDelegate, IOptions<CBCentralInitOptions> options, IDispatchQueueProvider dispatchQueueProvider, ITicker ticker)
    {
        ArgumentNullException.ThrowIfNull(cbCentralManagerDelegate);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);
        ArgumentNullException.ThrowIfNull(dispatchQueueProvider);
        ArgumentNullException.ThrowIfNull(ticker);

        _cbCentralManagerDelegate = cbCentralManagerDelegate;
        _options = options.Value;
        _dispatchQueueProvider = dispatchQueueProvider;
        _ticker = ticker;
    }

    /// <summary>
    ///     The underlying CBCentralManager instance.
    /// </summary>
    public CBCentralManager CbCentralManager
    {
        get
        {
            if (_cbCentralManager == null)
            {
                lock (_lock)
                {
                    if (_cbCentralManager == null)
                    {
                        _cbCentralManager = new CBCentralManager(this, _dispatchQueueProvider.GetCbCentralManagerDispatchQueue(), _options)
                        {
                            Delegate = this
                        };
                        _refreshSubscription = _ticker.Register("refresh_central_manager_properties", TimeSpan.FromSeconds(1), RefreshIsScanning, true);
                    }
                }
            }

            return _cbCentralManager;
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the central manager is currently scanning for peripherals.
    /// </summary>
    private bool CbCentralManagerIsScanning { get; set; }

    private void RefreshIsScanning()
    {
        if (_cbCentralManager == null)
        {
            return;
        }

        if (_cbCentralManager.IsScanning != CbCentralManagerIsScanning)
        {
            CbCentralManagerIsScanning = _cbCentralManager.IsScanning;
            if (CbCentralManagerIsScanning)
            {
                ScanningStarted();
            }
            else
            {
                ScanningStopped();
            }
        }
    }

    /// <summary>
    ///     Releases the unmanaged resources used by the CbCentralManagerWrapper and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshSubscription?.Dispose();
            _cbCentralManager?.StopScan();
            _cbCentralManager?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void ScanningStarted()
    {
        try
        {
            // ACTION
            _cbCentralManagerDelegate.ScanningStarted();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    private void ScanningStopped()
    {
        try
        {
            // ACTION
            _cbCentralManagerDelegate.ScanningStopped();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #region CBCentralManager

    /// <inheritdoc cref="CBCentralManagerDelegate.DiscoveredPeripheral" />

    // ReSharper disable once InconsistentNaming
    public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
    {
        try
        {
            _cbCentralManagerDelegate.DiscoveredPeripheral(peripheral, advertisementData, RSSI);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.UpdatedState" />
    public override void UpdatedState(CBCentralManager central)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(central);
            _cbCentralManagerDelegate.UpdatedState(central.State);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.WillRestoreState" />
    public override void WillRestoreState(CBCentralManager central, NSDictionary dict)
    {
        try
        {
            _cbCentralManagerDelegate.WillRestoreState(dict);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion

    #region CBPeripheral

    /// <inheritdoc cref="CBCentralManagerDelegate.FailedToConnectPeripheral" />
    public override void FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _cbCentralManagerDelegate.GetDevice(peripheral);

            // ACTION
            sharedDevice.FailedToConnectPeripheral(error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.DidDisconnectPeripheral" />
    public override void DidDisconnectPeripheral(CBCentralManager central,
        CBPeripheral peripheral,
        double timestamp,
        bool isReconnecting,
        NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _cbCentralManagerDelegate.GetDevice(peripheral);

            // ACTION
            sharedDevice.DidDisconnectPeripheral(timestamp, isReconnecting, error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.DisconnectedPeripheral" />
    public override void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _cbCentralManagerDelegate.GetDevice(peripheral);

            // ACTION
            sharedDevice.DisconnectedPeripheral(error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.ConnectedPeripheral" />
    public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _cbCentralManagerDelegate.GetDevice(peripheral);

            // ACTION
            sharedDevice.ConnectedPeripheral();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.ConnectionEventDidOccur" />
    public override void ConnectionEventDidOccur(CBCentralManager central, CBConnectionEvent connectionEvent, CBPeripheral peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _cbCentralManagerDelegate.GetDevice(peripheral);

            // ACTION
            sharedDevice.ConnectionEventDidOccur(connectionEvent);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.DidUpdateAncsAuthorization" />
    public override void DidUpdateAncsAuthorization(CBCentralManager central, CBPeripheral peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _cbCentralManagerDelegate.GetDevice(peripheral);

            // ACTION
            sharedDevice.DidUpdateAncsAuthorization();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion
}