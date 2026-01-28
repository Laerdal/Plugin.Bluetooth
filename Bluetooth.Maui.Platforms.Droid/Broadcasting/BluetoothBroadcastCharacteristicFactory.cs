namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastCharacteristicFactory : IBluetoothBroadcastCharacteristicFactory
{
    /// <inheritdoc/>
    public IBluetoothBroadcastCharacteristic CreateBroadcastCharacteristic(IBluetoothBroadcastService service, IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request)
    {
        return new BluetoothBroadcastCharacteristic(service, request);
    }
}
