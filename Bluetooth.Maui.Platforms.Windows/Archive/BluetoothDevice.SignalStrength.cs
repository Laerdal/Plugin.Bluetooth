namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    /// <inheritdoc/>
    /// <remarks>
    /// Not implemented on Windows. Signal strength reading is not currently supported on the Windows platform.
    /// </remarks>
    protected override void NativeReadSignalStrength()
    {
        // NativeReadSignalStrength not implemented on Windows yet.
    }

}
