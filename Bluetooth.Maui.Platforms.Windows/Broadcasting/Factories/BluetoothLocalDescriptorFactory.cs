using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Broadcasting.Factories;

/// <inheritdoc />
public class BluetoothLocalDescriptorFactory : BaseBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public override Abstractions.Broadcasting.IBluetoothLocalDescriptor CreateDescriptor(Abstractions.Broadcasting.IBluetoothLocalCharacteristic localCharacteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec)
    {
        throw new NotImplementedException();
    }
}
