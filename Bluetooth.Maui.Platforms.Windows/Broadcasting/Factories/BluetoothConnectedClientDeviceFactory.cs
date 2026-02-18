using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Broadcasting.Factories;

/// <inheritdoc />
public class BluetoothConnectedClientDeviceFactory : BaseBluetoothConnectedClientDeviceFactory
{
    /// <inheritdoc />
    public override IBluetoothConnectedDevice CreateConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec request)
    {
        throw new NotImplementedException();
    }
}
