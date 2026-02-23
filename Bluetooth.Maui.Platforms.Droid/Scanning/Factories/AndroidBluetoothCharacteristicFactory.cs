using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc />
    public AndroidBluetoothCharacteristicFactory(IBluetoothDescriptorFactory descriptorFactory)
        : base(descriptorFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteCharacteristic CreateCharacteristic(
        IBluetoothRemoteService remoteService,
        IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        return new AndroidBluetoothRemoteCharacteristic(remoteService, request, DescriptorFactory);
    }
}
