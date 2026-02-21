using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

/// <inheritdoc />
public class WindowsBluetoothLocalDescriptorFactory : BaseBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public override IBluetoothLocalDescriptor CreateDescriptor(IBluetoothLocalCharacteristic localCharacteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec)
    {
        throw new NotImplementedException();
    }
}