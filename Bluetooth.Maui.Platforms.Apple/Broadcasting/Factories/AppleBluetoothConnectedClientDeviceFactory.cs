using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public class AppleBluetoothConnectedClientDeviceFactory : BaseBluetoothConnectedClientDeviceFactory
{
    /// <inheritdoc />
    public override IBluetoothConnectedDevice CreateConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
    {
        return new AppleBluetoothConnectedDevice(broadcaster, spec);
    }
}
