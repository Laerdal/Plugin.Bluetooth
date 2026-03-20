namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothLocalDescriptorFactory : IBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public IBluetoothLocalDescriptor Create(IBluetoothLocalCharacteristic characteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec)
    {
        throw new NotImplementedException("Android GATT local descriptor creation is not yet implemented.");
    }
}
