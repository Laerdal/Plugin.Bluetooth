namespace Bluetooth.Maui.Platforms.DotNetCore.Scanning;

/// <inheritdoc/>
public class BluetoothCharacteristicFactory : IBluetoothCharacteristicFactory
{
    /// <inheritdoc/>
    public ValueTask<IBluetoothCharacteristic> CreateCharacteristicAsync(IBluetoothService service, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        return ValueTask.FromResult<IBluetoothCharacteristic>(new BluetoothCharacteristic(service, request));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
