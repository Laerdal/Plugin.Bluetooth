namespace Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

/// <inheritdoc />
public class WindowsBluetoothLocalDescriptorFactory : IBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public IBluetoothLocalDescriptor Create(IBluetoothLocalCharacteristic characteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec)
    {
        throw new NotImplementedException("Windows local descriptor factory implementation pending.");
    }
}
