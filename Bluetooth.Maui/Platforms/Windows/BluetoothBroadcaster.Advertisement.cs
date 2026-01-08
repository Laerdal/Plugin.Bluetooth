using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    protected override void NativeAdvertisementDataChanged()
    {
        throw new NotImplementedException();
    }
}
