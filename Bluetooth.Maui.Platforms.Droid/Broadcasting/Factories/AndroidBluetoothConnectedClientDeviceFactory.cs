using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothConnectedClientDeviceFactory : BaseBluetoothConnectedClientDeviceFactory
{
    /// <inheritdoc />
    public override IBluetoothConnectedDevice CreateConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
    {
        throw new NotImplementedException();
    }
}
