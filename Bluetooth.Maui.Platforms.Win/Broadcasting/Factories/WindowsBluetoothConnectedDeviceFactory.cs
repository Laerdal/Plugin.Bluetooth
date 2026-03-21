namespace Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

/// <inheritdoc />
public class WindowsBluetoothConnectedDeviceFactory : IBluetoothConnectedDeviceFactory
{
    /// <inheritdoc />
    public IBluetoothConnectedDevice Create(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
    {
        throw new NotSupportedException("Windows broadcaster connected-device creation is not implemented yet.");
    }
}
