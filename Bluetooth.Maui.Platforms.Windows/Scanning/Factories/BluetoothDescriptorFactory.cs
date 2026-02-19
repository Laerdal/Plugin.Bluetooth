using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <inheritdoc/>
public class BluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc/>
    public BluetoothDescriptorFactory()
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteDescriptor CreateDescriptor(
        Abstractions.Scanning.IBluetoothRemoteCharacteristic remoteCharacteristic,
        IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
    {
        return new BluetoothDescriptor(remoteCharacteristic, request);
    }
}
