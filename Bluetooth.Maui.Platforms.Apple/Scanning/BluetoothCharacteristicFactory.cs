namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc/>
public class BluetoothCharacteristicFactory : IBluetoothCharacteristicFactory
{
    /// <inheritdoc/>
    public IBluetoothCharacteristic CreateCharacteristic(IBluetoothService service, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        return new BluetoothCharacteristic(service, request);
    }
}
