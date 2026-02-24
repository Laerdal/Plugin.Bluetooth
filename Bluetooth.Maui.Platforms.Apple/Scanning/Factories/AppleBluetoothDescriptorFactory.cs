using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc />
    public override IBluetoothRemoteDescriptor Create(IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
    {
        return new AppleBluetoothRemoteDescriptor(remoteCharacteristic, spec);
    }
}
