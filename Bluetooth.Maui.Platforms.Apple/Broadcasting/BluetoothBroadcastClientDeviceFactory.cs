namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

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
