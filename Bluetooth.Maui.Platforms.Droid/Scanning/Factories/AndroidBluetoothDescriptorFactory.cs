using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc />
    public AndroidBluetoothDescriptorFactory()
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteDescriptor Create(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
    {
        return new AndroidBluetoothRemoteDescriptor(remoteCharacteristic, spec);
    }
}
