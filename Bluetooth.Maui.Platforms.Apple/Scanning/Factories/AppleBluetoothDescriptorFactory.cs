using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc />
    public override Abstractions.Scanning.IBluetoothRemoteDescriptor CreateDescriptor(Abstractions.Scanning.IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
    {
        return new AppleBluetoothRemoteDescriptor(remoteCharacteristic, request);
    }
}
