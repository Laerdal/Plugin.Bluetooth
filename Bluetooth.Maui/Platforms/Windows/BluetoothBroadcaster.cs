using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <inheritdoc cref="BaseBluetoothBroadcaster" />
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, BluetoothLeAdvertisementPublisherProxy.IBluetoothLeAdvertisementPublisherProxyDelegate, RadioProxy.IRadioProxyDelegate, BluetoothAdapterProxy.IBluetoothAdapterProxyDelegate
{

    public BluetoothLeAdvertisementPublisherProxy? BluetoothLeAdvertisementPublisherProxy { get; protected set; }


}
