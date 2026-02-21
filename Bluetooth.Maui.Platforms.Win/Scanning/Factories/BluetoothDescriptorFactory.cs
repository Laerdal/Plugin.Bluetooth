using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc />
public class BluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc />
    public BluetoothDescriptorFactory()
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteDescriptor CreateDescriptor(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
    {
        return new WindowsBluetoothRemoteDescriptor(remoteCharacteristic, request);
    }
}