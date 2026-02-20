using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class BluetoothConnectedClientDeviceFactory : BaseBluetoothConnectedClientDeviceFactory
{
    /// <inheritdoc />
    public override IBluetoothConnectedDevice CreateConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
    {
        throw new NotImplementedException();
    }
}