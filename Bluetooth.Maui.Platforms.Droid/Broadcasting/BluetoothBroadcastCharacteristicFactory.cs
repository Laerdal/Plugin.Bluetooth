namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastCharacteristicFactory : IBluetoothBroadcastCharacteristicFactory
{
    /// <inheritdoc/>
    public ValueTask<IBluetoothBroadcastCharacteristic> CreateBroadcastCharacteristicAsync(IBluetoothBroadcastService service, IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        return ValueTask.FromResult<IBluetoothBroadcastCharacteristic>(new BluetoothBroadcastCharacteristic(service, request));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
