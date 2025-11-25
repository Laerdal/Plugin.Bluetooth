using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

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
