using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothDevice : BaseBluetoothDevice, BluetoothLeDeviceProxy.IBluetoothLeDeviceProxyDelegate, GattSessionProxy.IGattSessionProxyDelegate
{
    public BluetoothLeDeviceProxy? BluetoothLeDeviceProxy { get; protected set; }

    public GattSessionProxy? GattSessionProxy { get; protected set; }

    public BluetoothDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer) : base(scanner, id, manufacturer)
    {
    }

    public BluetoothDevice(IBluetoothScanner scanner, BluetoothAdvertisement advertisement) : base(scanner, advertisement)
    {
    }

    public void OnGattServicesChanged()
    {
        // Placeholder for future implementation
    }

    public void OnNameChanged(string senderName)
    {
        CachedName = senderName;
    }

    public void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus)
    {
        // Placeholder for future implementation
    }

    public void OnCustomPairingRequested(DevicePairingRequestedEventArgs args)
    {
        // Placeholder for future implementation
    }

    public void OnMaxPduSizeChanged()
    {
        // Placeholder for future implementation
    }
}
