namespace Plugin.Bluetooth.Maui.PlatformSpecific;


/// <summary>
/// Proxy class for CoreBluetooth central manager delegate callbacks.
/// https://developer.apple.com/documentation/corebluetooth/cbcentralmanagerdelegate
/// </summary>
public partial class CbCentralManagerProxy : CBCentralManagerDelegate
{
    /// <summary>
    /// Initializes a new instance of the CbCentralManagerProxy class.
    /// </summary>
    /// <param name="cbCentralManagerProxyDelegate">The delegate to handle central manager proxy callbacks.</param>
    /// <param name="options">The initialization options for the central manager.</param>
    /// <param name="dispatchQueue">The dispatch queue for CoreBluetooth operations. If null, uses the main queue.</param>
    public CbCentralManagerProxy(ICbCentralManagerProxyDelegate cbCentralManagerProxyDelegate, CBCentralInitOptions? options = null, DispatchQueue? dispatchQueue = null)
    {
        CbCentralManagerProxyDelegate = cbCentralManagerProxyDelegate;
        dispatchQueue ??= DispatchQueue.DefaultGlobalQueue;
        options ??= new CBCentralInitOptions();
        CbCentralManager = new CBCentralManager(this, dispatchQueue, options)
        {
            Delegate = this
        };
    }

    /// <summary>
    /// Gets the delegate that handles central manager proxy callbacks.
    /// </summary>
    public ICbCentralManagerProxyDelegate CbCentralManagerProxyDelegate { get; }

    /// <summary>
    /// Gets the underlying CoreBluetooth central manager.
    /// </summary>
    public CBCentralManager CbCentralManager { get; }

    /// <summary>
    /// Releases the unmanaged resources used by the CbCentralManagerProxy and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CbCentralManager.Dispose();
        }

        base.Dispose(disposing);
    }

    #region CBCentralManager

    /// <inheritdoc cref="CBCentralManagerDelegate.DiscoveredPeripheral" />
    // ReSharper disable once InconsistentNaming
    public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
    {
        try
        {
            // ACTION
            CbCentralManagerProxyDelegate.DiscoveredPeripheral(peripheral, advertisementData, RSSI);
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
            // ACTION
            CbCentralManagerProxyDelegate.UpdatedState();
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
            // ACTION
            CbCentralManagerProxyDelegate.WillRestoreState(dict);
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
            var sharedDevice = CbCentralManagerProxyDelegate.GetDevice(peripheral);

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
            var sharedDevice = CbCentralManagerProxyDelegate.GetDevice(peripheral);

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
            var sharedDevice = CbCentralManagerProxyDelegate.GetDevice(peripheral);

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
            var sharedDevice = CbCentralManagerProxyDelegate.GetDevice(peripheral);

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
            var sharedDevice = CbCentralManagerProxyDelegate.GetDevice(peripheral);

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
            var sharedDevice = CbCentralManagerProxyDelegate.GetDevice(peripheral);

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

