namespace Bluetooth.Core.Broadcasting.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothConnectedDeviceFactory : IBluetoothConnectedDeviceFactory
{
    /// <inheritdoc />
    public abstract IBluetoothConnectedDevice Create(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec);
}
