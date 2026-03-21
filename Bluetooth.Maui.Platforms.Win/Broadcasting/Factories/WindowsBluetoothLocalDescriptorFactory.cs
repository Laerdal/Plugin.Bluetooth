namespace Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

/// <inheritdoc />
public class WindowsBluetoothLocalDescriptorFactory : IBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public IBluetoothLocalDescriptor Create(IBluetoothLocalCharacteristic characteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec)
    {
        throw new NotSupportedException("Windows local GATT descriptor creation is not implemented yet.");
    }
}
