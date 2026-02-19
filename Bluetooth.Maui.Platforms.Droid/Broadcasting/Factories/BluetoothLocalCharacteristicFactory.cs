using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class BluetoothLocalCharacteristicFactory : BaseBluetoothLocalCharacteristicFactory
{
    /// <inheritdoc />
    public BluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory) : base(localDescriptorFactory)
    {
    }

    /// <inheritdoc />
    public override Abstractions.Broadcasting.IBluetoothLocalCharacteristic CreateCharacteristic(Abstractions.Broadcasting.IBluetoothLocalService localService, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        throw new NotImplementedException();
    }
}
