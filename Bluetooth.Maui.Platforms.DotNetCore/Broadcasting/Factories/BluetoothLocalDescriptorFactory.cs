using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting.Factories;

/// <inheritdoc/>
public class BluetoothLocalDescriptorFactory : BaseBluetoothLocalDescriptorFactory
{
    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothLocalDescriptorFactory()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothLocalDescriptor CreateDescriptor(IBluetoothLocalCharacteristic localCharacteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec request)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
