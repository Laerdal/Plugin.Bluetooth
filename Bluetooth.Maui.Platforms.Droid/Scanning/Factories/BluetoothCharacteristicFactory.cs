using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class BluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothCharacteristicFactory(IBluetoothDescriptorFactory descriptorFactory) : base(descriptorFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothRemoteCharacteristic CreateCharacteristic(IBluetoothRemoteService remoteService, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}