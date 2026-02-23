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
    public override IBluetoothRemoteDescriptor CreateDescriptor(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
    {
        return new AndroidBluetoothRemoteDescriptor(remoteCharacteristic, request);
    }
}
