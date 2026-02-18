using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting.Factories;

/// <inheritdoc/>
public class BluetoothLocalCharacteristicFactory : BaseBluetoothLocalCharacteristicFactory
{
    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory) : base(localDescriptorFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothLocalCharacteristic CreateCharacteristic(IBluetoothLocalService localService, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
