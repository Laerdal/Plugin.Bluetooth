namespace Bluetooth.Maui.PlatformSpecific;


/// <summary>
/// Proxy class for CoreBluetooth peripheral manager delegate callbacks.
/// https://developer.apple.com/documentation/corebluetooth/cbperipheralmanagerdelegate
/// </summary>
public partial class CbPeripheralManagerProxy : CBPeripheralManagerDelegate
{
    /// <summary>
    /// Initializes a new instance of the CbPeripheralManagerProxy class.
    /// </summary>
    /// <param name="cbPeripheralManagerProxyDelegate">The delegate to handle peripheral manager proxy callbacks.</param>
    /// <param name="dispatchQueue">The dispatch queue for CoreBluetooth operations. If null, uses the default global queue.</param>
    public CbPeripheralManagerProxy(CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate cbPeripheralManagerProxyDelegate, DispatchQueue? dispatchQueue = null)
    {
        CbPeripheralManagerProxyDelegate = cbPeripheralManagerProxyDelegate;
        dispatchQueue ??= DispatchQueue.DefaultGlobalQueue;
        CbPeripheralManager = new CBPeripheralManager(this, dispatchQueue)
        {
            Delegate = this
        };
    }

    /// <summary>
    /// Gets the delegate that handles peripheral manager proxy callbacks.
    /// </summary>
    public CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate CbPeripheralManagerProxyDelegate { get; }

    /// <summary>
    /// Gets the underlying CoreBluetooth peripheral manager.
    /// </summary>
    public CBPeripheralManager CbPeripheralManager { get; }

    /// <summary>
    /// Releases the unmanaged resources used by the CbPeripheralManagerProxy and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CbPeripheralManager.Dispose();
        }

        base.Dispose(disposing);
    }

    #region CBPeripheralManagerDelegate

    /// <inheritdoc cref="CBPeripheralManagerDelegate.StateUpdated" />
    public override void StateUpdated(CBPeripheralManager peripheral)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.StateUpdated();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.AdvertisingStarted" />
    public override void AdvertisingStarted(CBPeripheralManager peripheral, NSError? error)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.AdvertisingStarted(error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.CharacteristicSubscribed" />
    public override void CharacteristicSubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.CharacteristicSubscribed(central, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.CharacteristicUnsubscribed" />
    public override void CharacteristicUnsubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.CharacteristicUnsubscribed(central, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.ServiceAdded" />
    public override void ServiceAdded(CBPeripheralManager peripheral, CBService service, NSError? error)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.ServiceAdded(service);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.ReadRequestReceived" />
    public override void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.ReadRequestReceived(request);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.WillRestoreState" />
    public override void WillRestoreState(CBPeripheralManager peripheral, NSDictionary dict)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.WillRestoreState(dict);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.WriteRequestsReceived" />
    public override void WriteRequestsReceived(CBPeripheralManager peripheral, CBATTRequest[] requests)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.WriteRequestsReceived(requests);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.DidOpenL2CapChannel" />
    public override void DidOpenL2CapChannel(CBPeripheralManager peripheral, CBL2CapChannel? channel, NSError? error)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.DidOpenL2CapChannel(error, channel);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.DidPublishL2CapChannel" />
    public override void DidPublishL2CapChannel(CBPeripheralManager peripheral, ushort psm, NSError? error)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.DidPublishL2CapChannel(error, psm);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.DidUnpublishL2CapChannel" />
    public override void DidUnpublishL2CapChannel(CBPeripheralManager peripheral, ushort psm, NSError? error)
    {
        try
        {
            // ACTION
            CbPeripheralManagerProxyDelegate.DidUnpublishL2CapChannel(error, psm);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion
}

