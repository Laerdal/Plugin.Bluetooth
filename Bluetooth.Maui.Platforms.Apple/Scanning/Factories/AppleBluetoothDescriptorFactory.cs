using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc />
    public override IBluetoothRemoteDescriptor CreateDescriptor(IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
    {
        return new AppleBluetoothRemoteDescriptor(remoteCharacteristic, request);
    }
}