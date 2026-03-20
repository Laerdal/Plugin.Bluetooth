// TODO: Uncomment when Core factory infrastructure exists
// using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

/// <inheritdoc />
public class WindowsBluetoothConnectedDeviceFactory : IBluetoothConnectedDeviceFactory
{
    /// <inheritdoc />
    public IBluetoothConnectedDevice Create(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
    {
        throw new NotImplementedException("Windows connected device factory implementation pending.");
    }
}
