namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothLocalCharacteristicFactory : IBluetoothLocalCharacteristicFactory
{
    /// <inheritdoc />
    public IBluetoothLocalCharacteristic Create(IBluetoothLocalService service, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        return new AndroidBluetoothLocalCharacteristic(service, spec);
    }
}
