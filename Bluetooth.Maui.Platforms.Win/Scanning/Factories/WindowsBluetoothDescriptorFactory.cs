using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothDescriptorFactory()
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteDescriptor Create(
        Abstractions.Scanning.IBluetoothRemoteCharacteristic remoteCharacteristic,
        IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
    {
        return new WindowsBluetoothRemoteDescriptor(remoteCharacteristic, spec);
    }
}
