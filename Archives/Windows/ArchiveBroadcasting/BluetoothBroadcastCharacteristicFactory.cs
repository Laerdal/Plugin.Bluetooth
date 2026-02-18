using Bluetooth.Abstractions.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastCharacteristicFactory : IBluetoothBroadcastCharacteristicFactory
{
    /// <inheritdoc/>
    public IBluetoothBroadcastCharacteristic CreateCharacteristic(IBluetoothBroadcastService service, IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request)
    {
        return new BluetoothBroadcastCharacteristic(service, request);
    }
}
