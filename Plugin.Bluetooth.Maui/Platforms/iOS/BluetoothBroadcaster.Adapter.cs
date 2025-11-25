using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothBroadcaster
{

    public void StateUpdated()
    {
        throw new NotImplementedException();
    }

    protected override ValueTask NativeInitializeAsync()
    {
        throw new NotImplementedException();
    }

    protected override void NativeRefreshIsBluetoothOn()
    {
        throw new NotImplementedException();
    }
}
