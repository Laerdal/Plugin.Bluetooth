using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Broadcasting.Factories;

/// <inheritdoc />
public class BluetoothLocalCharacteristicFactory : BaseBluetoothLocalCharacteristicFactory
{
    /// <inheritdoc />
    public BluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory) : base(localDescriptorFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothLocalCharacteristic CreateCharacteristic(IBluetoothLocalService localService, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        throw new NotImplementedException();
    }
}