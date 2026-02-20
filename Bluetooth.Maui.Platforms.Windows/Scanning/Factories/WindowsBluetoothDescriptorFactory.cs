using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothDescriptorFactory()
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteDescriptor CreateDescriptor(
        Abstractions.Scanning.IBluetoothRemoteCharacteristic remoteCharacteristic,
        IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
    {
        return new WindowsBluetoothRemoteDescriptor(remoteCharacteristic, request);
    }
}
