using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    public BluetoothGattServerCallbackProxy.IDevice GetDevice(Android.Bluetooth.BluetoothDevice? native)
    {
        throw new NotImplementedException();
    }
}
