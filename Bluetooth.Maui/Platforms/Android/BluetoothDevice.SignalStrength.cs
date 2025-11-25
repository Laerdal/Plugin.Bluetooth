using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    public void OnReadRemoteRssi(GattStatus status, int rssi)
    {
        try
        {
            AndroidNativeGattStatusException.ThrowIfNotSuccess(status);
            OnSignalStrengthRead(rssi);
        }
        catch (Exception e)
        {
            OnSignalStrengthReadFailed(e);
        }
    }

    protected override void NativeReadSignalStrength()
    {
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);
        BluetoothGattProxy.BluetoothGatt.ReadRemoteRssi();
    }
}
