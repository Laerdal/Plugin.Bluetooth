namespace Bluetooth.Core.Broadcasting.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothConnectedClientDeviceFactory : IBluetoothConnectedDeviceFactory
{
    /// <inheritdoc />
    public abstract IBluetoothConnectedDevice CreateConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec);
}