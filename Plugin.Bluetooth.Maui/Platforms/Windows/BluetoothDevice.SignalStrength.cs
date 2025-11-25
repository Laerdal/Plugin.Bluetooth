using System.Diagnostics;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothDevice
{
    protected override void NativeReadSignalStrength()
    {
        // NativeReadSignalStrength not implemented on Windows yet.
    }

}
