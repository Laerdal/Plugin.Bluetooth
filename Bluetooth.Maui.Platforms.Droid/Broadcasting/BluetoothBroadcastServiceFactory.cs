namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastServiceFactory : IBluetoothBroadcastServiceFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothBroadcastServiceFactory"/> class.
    /// </summary>
    public BluetoothBroadcastServiceFactory(IBluetoothBroadcastCharacteristicFactory characteristicFactory)
    {
        CharacteristicFactory = characteristicFactory;
    }

    private IBluetoothBroadcastCharacteristicFactory CharacteristicFactory { get; }

    /// <inheritdoc/>
    public ValueTask<IBluetoothBroadcastService> CreateBroadcastServiceAsync(IBluetoothBroadcaster broadcaster,
        IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest request,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        return ValueTask.FromResult<IBluetoothBroadcastService>(new BluetoothBroadcastService(broadcaster, request, CharacteristicFactory));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
