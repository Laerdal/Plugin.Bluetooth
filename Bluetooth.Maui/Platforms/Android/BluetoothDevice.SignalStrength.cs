using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    /// <summary>
    /// Called when a remote RSSI read operation completes on the Android platform.
    /// </summary>
    /// <param name="status">The status of the RSSI read operation.</param>
    /// <param name="rssi">The RSSI value in dBm.</param>
    /// <exception cref="AndroidNativeGattStatusException">Thrown when the status indicates an error.</exception>
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

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when BluetoothGattProxy is <c>null</c>.</exception>
    protected override void NativeReadSignalStrength()
    {
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);
        BluetoothGattProxy.BluetoothGatt.ReadRemoteRssi();
    }
}
