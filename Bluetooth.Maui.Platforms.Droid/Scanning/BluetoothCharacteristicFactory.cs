namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc/>
public class BluetoothCharacteristicFactory : IBluetoothCharacteristicFactory
{
    /// <inheritdoc/>
    public IBluetoothCharacteristic CreateCharacteristic(IBluetoothService service, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        return new BluetoothCharacteristic(service, request);
    }
}
