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

    #region BaseBluetoothDevice

    protected override void NativeServicesExploration()
    {
        throw new NotImplementedException();
    }

    protected override void NativeRefreshIsConnected()
    {
        throw new NotImplementedException();
    }

    protected override void NativeConnect()
    {
        throw new NotImplementedException();
    }

    protected override void NativeDisconnect()
    {
        throw new NotImplementedException();
    }

    protected override void NativeReadSignalStrength()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region BluetoothLeDeviceProxy.IBluetoothLeDeviceProxyDelegate

    public void OnGattServicesChanged()
    {
        throw new NotImplementedException();
    }

    public void OnConnectionStatusChanged(BluetoothConnectionStatus newConnectionStatus)
    {
        throw new NotImplementedException();
    }

    public void OnNameChanged(string senderName)
    {
        throw new NotImplementedException();
    }

    public void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus)
    {
        throw new NotImplementedException();
    }

    public void OnCustomPairingRequested(DevicePairingRequestedEventArgs args)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region GattSessionProxy.IGattSessionProxyDelegate

    public void OnGattSessionStatusChanged(GattSessionStatus argsStatus)
    {
        throw new NotImplementedException();
    }

    public void OnMaxPduSizeChanged()
    {
        throw new NotImplementedException();
    }

    #endregion

}
