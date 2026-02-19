using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public class AppleBluetoothLocalCharacteristicFactory : BaseBluetoothLocalCharacteristicFactory
{
    /// <inheritdoc />
    public AppleBluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory) : base(localDescriptorFactory)
    {
    }

    /// <inheritdoc />
    public override Abstractions.Broadcasting.IBluetoothLocalCharacteristic CreateCharacteristic(Abstractions.Broadcasting.IBluetoothLocalService localService, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        return new AppleBluetoothLocalCharacteristic(localService, spec, LocalDescriptorFactory);
    }
}
