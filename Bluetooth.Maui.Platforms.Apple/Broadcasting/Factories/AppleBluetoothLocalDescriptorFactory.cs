using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public class AppleBluetoothLocalDescriptorFactory : BaseBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public override IBluetoothLocalDescriptor CreateDescriptor(IBluetoothLocalCharacteristic localCharacteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec)
    {
        return new AppleBluetoothLocalDescriptor(localCharacteristic, spec);
    }
}
