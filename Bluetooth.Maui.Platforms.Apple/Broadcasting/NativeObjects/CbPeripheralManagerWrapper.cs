namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

/// <summary>
/// Proxy class for CoreBluetooth peripheral manager delegate callbacks.
/// https://developer.apple.com/documentation/corebluetooth/cbperipheralmanagerdelegate
/// </summary>
public partial class CbPeripheralManagerWrapper : CBPeripheralManagerDelegate
{
    /// <summary>
    /// The underlying CBPeripheralManager instance.
    /// </summary>
    public CBPeripheralManager CbPeripheralManager { get; }

    /// <summary>
    /// The delegate proxy for handling peripheral manager events.
    /// </summary>
    private readonly CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate _broadcaster;

    /// <summary>
    /// Initializes a new instance of the CbPeripheralManagerWrapper class.
    /// </summary>
    /// <param name="broadcaster">The delegate proxy for handling peripheral manager events.</param>
    /// <param name="queue">The dispatch queue for peripheral manager events.</param>
    /// <param name="options">The initialization options for the peripheral manager.</param>
    public CbPeripheralManagerWrapper(CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate broadcaster, DispatchQueue queue, CbPeripheralManagerOptions options)
    {
        _broadcaster = broadcaster;
        CbPeripheralManager = new CBPeripheralManager(this, queue, options)
        {
            Delegate = this
        };
    }

    /// <summary>
    /// Releases the unmanaged resources used by the CbCentralManagerWrapper and optionally releases the managed resources.
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
            ArgumentNullException.ThrowIfNull(peripheral);
            _broadcaster.StateUpdated(peripheral.State);
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
            _broadcaster.AdvertisingStarted(error);
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
            ArgumentNullException.ThrowIfNull(characteristic);
            ArgumentNullException.ThrowIfNull(characteristic.Service, nameof(characteristic.Service));

            // GET SERVICE
            var sharedService = _broadcaster.GetService(characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.CharacteristicSubscribed(central, characteristic);
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
            ArgumentNullException.ThrowIfNull(characteristic);
            ArgumentNullException.ThrowIfNull(characteristic.Service, nameof(characteristic.Service));

            // GET SERVICE
            var sharedService = _broadcaster.GetService(characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.CharacteristicUnsubscribed(central, characteristic);
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
            _broadcaster.ServiceAdded(service);
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
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Characteristic);
            ArgumentNullException.ThrowIfNull(request.Characteristic.Service, nameof(request.Characteristic.Service));

            // GET SERVICE
            var sharedService = _broadcaster.GetService(request.Characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(request.Characteristic);

            // ACTION
            sharedCharacteristic.ReadRequestReceived(request);
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
            _broadcaster.WillRestoreState(dict);
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
            ArgumentNullException.ThrowIfNull(requests);
            foreach (var request in requests)
            {
                ArgumentNullException.ThrowIfNull(request);
                ArgumentNullException.ThrowIfNull(request.Characteristic);
                ArgumentNullException.ThrowIfNull(request.Characteristic.Service, nameof(request.Characteristic.Service));

                // GET SERVICE
                var sharedService = _broadcaster.GetService(request.Characteristic.Service);

                // GET CHARACTERISTIC
                var sharedCharacteristic = sharedService.GetCharacteristic(request.Characteristic);

                // ACTION
                sharedCharacteristic.WriteRequestsReceived(request);
            }
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
            _broadcaster.DidOpenL2CapChannel(error, channel);
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
            _broadcaster.DidPublishL2CapChannel(error, psm);
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
            _broadcaster.DidUnpublishL2CapChannel(error, psm);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion
}

