namespace Bluetooth.Maui.PlatformSpecific;


/// <summary>
/// Proxy class for CoreBluetooth peripheral delegate callbacks.
/// https://developer.apple.com/documentation/corebluetooth/cbperipheraldelegate
/// </summary>
public sealed partial class CbPeripheralProxy : CBPeripheralDelegate
{
    /// <summary>
    /// Initializes a new instance of the CbPeripheralProxy class.
    /// </summary>
    /// <param name="cbPeripheralProxyDelegate">The delegate to handle peripheral proxy callbacks.</param>
    /// <param name="cbPeripheral">The CoreBluetooth peripheral to proxy.</param>
    public CbPeripheralProxy(ICbPeripheralProxyDelegate cbPeripheralProxyDelegate, CBPeripheral cbPeripheral)
    {
        CbPeripheralProxyDelegate = cbPeripheralProxyDelegate;
        CbPeripheral = cbPeripheral;
        CbPeripheral.Delegate = this;
    }

    /// <summary>
    /// Gets the delegate that handles peripheral proxy callbacks.
    /// </summary>
    public ICbPeripheralProxyDelegate CbPeripheralProxyDelegate { get; }

    /// <summary>
    /// Gets the underlying CoreBluetooth peripheral.
    /// </summary>
    public CBPeripheral CbPeripheral { get; }

    #region CBPeripheral

    /// <inheritdoc cref="CBPeripheralDelegate.DiscoveredService" />
    public override void DiscoveredService(CBPeripheral peripheral, NSError? error)
    {
        try
        {
            // ACTION
            CbPeripheralProxyDelegate.DiscoveredService(error);
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
            CbPeripheralProxyDelegate.RssiRead(error, rssi);
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
            CbPeripheralProxyDelegate.RssiUpdated(error);
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
            CbPeripheralProxyDelegate.UpdatedName();
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
            CbPeripheralProxyDelegate.DidOpenL2CapChannel(error, channel);
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
            CbPeripheralProxyDelegate.IsReadyToSendWriteWithoutResponse();
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
            var sharedService = CbPeripheralProxyDelegate.GetService(service);

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
            CbPeripheralProxyDelegate.ModifiedServices(services);
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
            var sharedService = CbPeripheralProxyDelegate.GetService(service);

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
            var sharedService = CbPeripheralProxyDelegate.GetService(characteristic.Service);

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
            var sharedService = CbPeripheralProxyDelegate.GetService(characteristic.Service);

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
            var sharedService = CbPeripheralProxyDelegate.GetService(characteristic.Service);

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
            var sharedService = CbPeripheralProxyDelegate.GetService(characteristic.Service);

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
            var sharedService = CbPeripheralProxyDelegate.GetService(descriptor.Characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor.Characteristic);

            // ACTION
            sharedCharacteristic.UpdatedValue(error, descriptor);
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
            var sharedService = CbPeripheralProxyDelegate.GetService(descriptor.Characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor.Characteristic);

            // ACTION
            sharedCharacteristic.WroteDescriptorValue(error, descriptor);
        }

        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion
}
