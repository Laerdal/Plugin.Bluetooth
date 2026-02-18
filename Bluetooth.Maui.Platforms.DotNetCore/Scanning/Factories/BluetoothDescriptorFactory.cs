using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Scanning.Factories;

/// <inheritdoc/>
public class BluetoothDescriptorFactory : BaseBluetoothDescriptorFactory
{
    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothDescriptorFactory()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override Abstractions.Scanning.IBluetoothRemoteDescriptor CreateDescriptor(Abstractions.Scanning.IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
