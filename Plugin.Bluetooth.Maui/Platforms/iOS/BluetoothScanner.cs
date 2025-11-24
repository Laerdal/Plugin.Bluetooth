using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothScanner : BaseBluetoothScanner, CbCentralManagerProxy.ICbCentralManagerProxyDelegate
{
    public CbCentralManagerProxy? CbCentralManagerProxy { get; protected set; }


    #region CbCentralManagerProxy

    public void WillRestoreState(NSDictionary dict)
    {
        // Placeholder for future implementation if needed
    }

    public CbCentralManagerProxy.ICbPeripheralDelegate GetDevice(CBPeripheral peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            var match = Devices.OfType<BluetoothDevice>().SingleOrDefault(device => device.CbPeripheralDelegateProxy.CbPeripheral.Identifier.ToString() == peripheral.Identifier.ToString() && device.CbPeripheralDelegateProxy.CbPeripheral.Handle.Handle == peripheral.Handle.Handle);

            if (match == null)
            {
                throw new DeviceNotFoundException(this, peripheral.Identifier.ToString());
            }

            return match;
        }
        catch (InvalidOperationException e)
        {
            var matches = Devices.OfType<BluetoothDevice>().Where(device => device.CbPeripheralDelegateProxy.CbPeripheral.Identifier.ToString() == peripheral.Identifier.ToString() && device.CbPeripheralDelegateProxy.CbPeripheral.Handle.Handle == peripheral.Handle.Handle).ToArray();
            throw new MultipleDevicesFoundException(this, matches, e);
        }
    }

    #endregion

}
