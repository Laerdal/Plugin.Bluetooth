namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;


/// <summary>
/// Proxy class for CoreBluetooth peripheral delegate callbacks.
/// https://developer.apple.com/documentation/corebluetooth/cbperipheraldelegate
/// </summary>
public partial class CbPeripheralWrapper : CBPeripheralDelegate
{
    /// <summary>
    /// Gets the underlying CoreBluetooth peripheral.
    /// </summary>
    public CBPeripheral CbPeripheral { get; }

    /// <summary>
    /// Gets the delegate that handles peripheral proxy callbacks.
    /// </summary>
    private readonly CbPeripheralWrapper.ICbPeripheralDelegate _cbPeripheralDelegate;

    /// <summary>
    /// Initializes a new instance of the CbPeripheralManager class.
    /// </summary>
    /// <param name="cbPeripheralDelegate">The delegate to handle peripheral proxy callbacks.</param>
    /// <param name="cbPeripheral">The CoreBluetooth peripheral to proxy.</param>
    public CbPeripheralWrapper(ICbPeripheralDelegate cbPeripheralDelegate, CBPeripheral cbPeripheral)
    {
        _cbPeripheralDelegate = cbPeripheralDelegate;
        CbPeripheral = cbPeripheral;
        CbPeripheral.Delegate = this;
    }


    /// <summary>
    /// Releases the unmanaged resources used by the CbCentralManagerWrapper and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CbPeripheral.Dispose();
        }

        base.Dispose(disposing);
    }

    #region CBPeripheral

    /// <inheritdoc cref="CBPeripheralDelegate.DiscoveredService" />
    public override void DiscoveredService(CBPeripheral peripheral, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralDelegate.DiscoveredService(error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.RssiRead" />
    public override void RssiRead(CBPeripheral peripheral, NSNumber rssi, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralDelegate.RssiRead(error, rssi);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.RssiUpdated" />
    public override void RssiUpdated(CBPeripheral peripheral, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralDelegate.RssiUpdated(error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.UpdatedName" />
    public override void UpdatedName(CBPeripheral peripheral)
    {
        try
        {
            // ACTION
            _cbPeripheralDelegate.UpdatedName();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.DidOpenL2CapChannel" />
    public override void DidOpenL2CapChannel(CBPeripheral peripheral, CBL2CapChannel? channel, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralDelegate.DidOpenL2CapChannel(error, channel);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.IsReadyToSendWriteWithoutResponse" />
    public override void IsReadyToSendWriteWithoutResponse(CBPeripheral peripheral)
    {
        try
        {
            // ACTION
            _cbPeripheralDelegate.IsReadyToSendWriteWithoutResponse();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion

    #region CBService

    /// <inheritdoc cref="CBPeripheralDelegate.DiscoveredCharacteristics" />
    public override void DiscoveredCharacteristics(CBPeripheral peripheral, CBService service, NSError? error)
    {
        try
        {
            // GET SERVICE
            var sharedService = _cbPeripheralDelegate.GetService(service);

            // ACTION
            sharedService.DiscoveredCharacteristics(error, service);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.ModifiedServices" />
    public override void ModifiedServices(CBPeripheral peripheral, CBService[] services)
    {
        try
        {
            // ACTION
            _cbPeripheralDelegate.ModifiedServices(services);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.DiscoveredIncludedService" />
    public override void DiscoveredIncludedService(CBPeripheral peripheral, CBService service, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(service);

            // GET SERVICE
            var sharedService = _cbPeripheralDelegate.GetService(service);

            // ACTION
            sharedService.DiscoveredIncludedService(error, service);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion

    #region CBCharacteristic

    /// <inheritdoc cref="CBPeripheralDelegate.DiscoveredDescriptor" />
    public override void DiscoveredDescriptor(CBPeripheral peripheral, CBCharacteristic characteristic, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            ArgumentNullException.ThrowIfNull(characteristic.Service, nameof(characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralDelegate.GetService(characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.DiscoveredDescriptor(error, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.WroteCharacteristicValue" />
    public override void WroteCharacteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            ArgumentNullException.ThrowIfNull(characteristic.Service, nameof(characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralDelegate.GetService(characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.WroteCharacteristicValue(error, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.UpdatedCharacterteristicValue" />
    public override void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            ArgumentNullException.ThrowIfNull(characteristic.Service, nameof(characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralDelegate.GetService(characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.UpdatedCharacteristicValue(error, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.UpdatedNotificationState" />
    public override void UpdatedNotificationState(CBPeripheral peripheral, CBCharacteristic characteristic, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            ArgumentNullException.ThrowIfNull(characteristic.Service, nameof(characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralDelegate.GetService(characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.UpdatedNotificationState(error, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion

    #region CBDescriptor

    /// <inheritdoc cref="CBPeripheralDelegate.UpdatedValue" />
    public override void UpdatedValue(CBPeripheral peripheral, CBDescriptor descriptor, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(descriptor);
            ArgumentNullException.ThrowIfNull(descriptor.Characteristic, nameof(descriptor.Characteristic));
            ArgumentNullException.ThrowIfNull(descriptor.Characteristic.Service, nameof(descriptor.Characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralDelegate.GetService(descriptor.Characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor.Characteristic);

            // GET DESCRIPTOR
            var sharedDescriptor = sharedCharacteristic.GetDescriptor(descriptor);

            // ACTION
            sharedDescriptor.UpdatedValue(error, descriptor);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralDelegate.WroteDescriptorValue" />
    public override void WroteDescriptorValue(CBPeripheral peripheral, CBDescriptor descriptor, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(descriptor);
            ArgumentNullException.ThrowIfNull(descriptor.Characteristic, nameof(descriptor.Characteristic));
            ArgumentNullException.ThrowIfNull(descriptor.Characteristic.Service, nameof(descriptor.Characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralDelegate.GetService(descriptor.Characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor.Characteristic);

            // GET DESCRIPTOR
            var sharedDescriptor = sharedCharacteristic.GetDescriptor(descriptor);

            // ACTION
            sharedDescriptor.WroteDescriptorValue(error, descriptor);
        }

        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion
}
