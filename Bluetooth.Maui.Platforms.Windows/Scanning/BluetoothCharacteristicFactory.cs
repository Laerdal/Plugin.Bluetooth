using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <inheritdoc/>
public class BluetoothCharacteristicFactory : IBluetoothCharacteristicFactory
{
    /// <inheritdoc/>
    public IBluetoothCharacteristic CreateCharacteristic(IBluetoothService service, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        return new BluetoothCharacteristic(service, request);
    }
}
