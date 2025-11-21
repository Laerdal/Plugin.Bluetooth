using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, AdvertiseCallbackProxy.IBroadcaster, BluetoothGattServerCallbackProxy.IBluetoothGattServerCallbackProxyDelegate
{
    public BluetoothGattServerCallbackProxy? BluetoothGattServerCallbackProxy { get; protected set; }

    public AdvertiseCallbackProxy AdvertiseCallbackProxy { get; }

    public BluetoothBroadcaster()
    {
        AdvertiseCallbackProxy = new AdvertiseCallbackProxy(this);
    }

    #region BluetoothGattServerCallbackProxy.IBluetoothGattServerCallbackProxyDelegate

    public void OnMtuChanged(int mtu)
    {
        throw new NotImplementedException();
    }

    public void OnExecuteWrite(int requestId, bool execute)
    {
        throw new NotImplementedException();
    }

    public void OnNotificationSent(GattStatus status)
    {
        throw new NotImplementedException();
    }

    public void OnPhyRead(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy)
    {
        throw new NotImplementedException();
    }

    public void OnPhyUpdate(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy)
    {
        throw new NotImplementedException();
    }

    public void OnServiceAdded(GattStatus status, BluetoothGattService? service)
    {
        throw new NotImplementedException();
    }

    public void OnConnectionStateChange(ProfileState status, ProfileState newState)
    {
        throw new NotImplementedException();
    }

    public void OnCharacteristicReadRequest(int requestId, int offset, BluetoothGattCharacteristic? characteristic)
    {
        throw new NotImplementedException();
    }

    public void OnCharacteristicWriteRequest(int requestId,
        BluetoothGattCharacteristic? characteristic,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[]? value)
    {
        throw new NotImplementedException();
    }

    public void OnDescriptorReadRequest(int requestId, int offset, BluetoothGattDescriptor? descriptor)
    {
        throw new NotImplementedException();
    }

    public void OnDescriptorWriteRequest(int requestId,
        BluetoothGattDescriptor? descriptor,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[]? value)
    {
        throw new NotImplementedException();
    }

    #endregion

}
