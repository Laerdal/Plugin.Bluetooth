using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <inheritdoc cref="BaseBluetoothBroadcaster" />
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate
{

    public CbPeripheralManagerProxy? CbPeripheralManagerProxy { get; protected set; }

    #region CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate


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

    #endregion

}
