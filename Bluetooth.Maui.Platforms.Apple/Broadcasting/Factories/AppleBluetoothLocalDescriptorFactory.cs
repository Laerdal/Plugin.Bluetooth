using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public class AppleBluetoothLocalDescriptorFactory : BaseBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public override Abstractions.Broadcasting.IBluetoothLocalDescriptor CreateDescriptor(Abstractions.Broadcasting.IBluetoothLocalCharacteristic localCharacteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec)
    {
        return new AppleBluetoothLocalDescriptor(localCharacteristic, spec);
    }
}
