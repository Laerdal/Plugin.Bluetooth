using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothLocalCharacteristicFactory : BaseBluetoothLocalCharacteristicFactory
{
    /// <inheritdoc />
    public AndroidBluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory) : base(localDescriptorFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothLocalCharacteristic CreateCharacteristic(IBluetoothLocalService localService, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        throw new NotImplementedException();
    }
}
