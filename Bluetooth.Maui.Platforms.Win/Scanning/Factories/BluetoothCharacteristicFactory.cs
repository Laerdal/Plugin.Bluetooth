using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc />
public class BluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc />
    public BluetoothCharacteristicFactory(IBluetoothDescriptorFactory descriptorFactory) : base(descriptorFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteCharacteristic CreateCharacteristic(
        IBluetoothRemoteService remoteService,
        IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        return new WindowsBluetoothRemoteCharacteristic(remoteService, request, DescriptorFactory);
    }
}