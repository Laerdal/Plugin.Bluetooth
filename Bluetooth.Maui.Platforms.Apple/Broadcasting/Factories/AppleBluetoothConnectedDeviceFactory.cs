using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public class AppleBluetoothConnectedDeviceFactory : BaseBluetoothConnectedDeviceFactory
{
    /// <inheritdoc />
    public override IBluetoothConnectedDevice Create(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
    {
        return new AppleBluetoothConnectedDevice(broadcaster, spec);
    }
}
