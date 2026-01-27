namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastCharacteristic : BaseBluetoothBroadcastCharacteristic
{
    /// <inheritdoc/>
    public BluetoothBroadcastCharacteristic(IBluetoothBroadcastService service, IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request) : base(service, request)
    {
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override Task NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}