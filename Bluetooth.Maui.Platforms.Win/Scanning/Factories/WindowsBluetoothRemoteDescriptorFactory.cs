// TODO: Uncomment when Core factory infrastructure exists
// using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc />
public class WindowsBluetoothRemoteDescriptorFactory : IBluetoothRemoteDescriptorFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothRemoteDescriptorFactory()
    {
    }

    /// <inheritdoc/>
    public Abstractions.Scanning.IBluetoothRemoteDescriptor Create(
        Abstractions.Scanning.IBluetoothRemoteCharacteristic characteristic,
        IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
    {
        return new WindowsBluetoothRemoteDescriptor(characteristic, spec);
    }
}
