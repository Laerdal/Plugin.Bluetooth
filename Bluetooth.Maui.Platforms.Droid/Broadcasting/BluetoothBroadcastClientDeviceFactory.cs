namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastClientDeviceFactory : IBluetoothBroadcastClientDeviceFactory
{
    /// <inheritdoc/>
    public IBluetoothBroadcastClientDevice CreateBroadcastClientDevice(IBluetoothBroadcaster bluetoothBroadcaster,
        IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest request)
    {
        return new BluetoothBroadcastClientDevice(bluetoothBroadcaster, request);
    }
}
