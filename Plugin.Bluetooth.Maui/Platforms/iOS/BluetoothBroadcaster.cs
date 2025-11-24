using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate
{

    public CbPeripheralManagerProxy? CbPeripheralManagerProxy { get; protected set; }

    #region BaseBluetoothBroadcaster

    protected override void NativeRefreshIsBluetoothOn()
    {
        throw new NotImplementedException();
    }

    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException();
    }

    protected override void NativeStart()
    {
        throw new NotImplementedException();
    }

    protected override void NativeStop()
    {
        throw new NotImplementedException();
    }

    protected async override ValueTask NativeInitializeAsync()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate

    public void AdvertisingStarted(NSError? error)
    {
        throw new NotImplementedException();
    }

    public void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        throw new NotImplementedException();
    }

    public void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        throw new NotImplementedException();
    }

    public void ServiceAdded(CBService service)
    {
        throw new NotImplementedException();
    }

    public void ReadRequestReceived(CBATTRequest request)
    {
        throw new NotImplementedException();
    }

    public void WillRestoreState(NSDictionary dict)
    {
        throw new NotImplementedException();
    }

    public void WriteRequestsReceived(CBATTRequest[] requests)
    {
        throw new NotImplementedException();
    }

    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        throw new NotImplementedException();
    }

    public void DidPublishL2CapChannel(NSError? error, ushort psm)
    {
        throw new NotImplementedException();
    }

    public void DidUnpublishL2CapChannel(NSError? error, ushort psm)
    {
        throw new NotImplementedException();
    }

    public void StateUpdated()
    {
        throw new NotImplementedException();
    }

    #endregion

}
