namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothRemoteDescriptorFactory : IBluetoothRemoteDescriptorFactory
{
    /// <inheritdoc />
    public AndroidBluetoothRemoteDescriptorFactory()
    {
    }

    /// <inheritdoc />
    public IBluetoothRemoteDescriptor Create(
        IBluetoothRemoteCharacteristic characteristic,
        IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
    {
        return new AndroidBluetoothRemoteDescriptor(characteristic, spec);
    }
}
