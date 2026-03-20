namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothConnectedDeviceFactory : IBluetoothConnectedDeviceFactory
{
    /// <inheritdoc />
    public IBluetoothConnectedDevice Create(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
    {
        return new AndroidBluetoothConnectedDevice(broadcaster, spec);
    }
}
