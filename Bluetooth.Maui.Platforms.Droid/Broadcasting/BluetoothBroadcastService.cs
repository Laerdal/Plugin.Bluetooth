namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastService : BaseBluetoothBroadcastService
{

    /// <inheritdoc/>
    public BluetoothBroadcastService(IBluetoothBroadcaster broadcaster,
        IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest request,
        IBluetoothBroadcastCharacteristicFactory characteristicFactory) : base(broadcaster, request, characteristicFactory)
    {
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
