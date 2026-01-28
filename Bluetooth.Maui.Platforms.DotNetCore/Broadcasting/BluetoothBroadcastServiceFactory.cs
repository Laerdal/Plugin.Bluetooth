namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

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
    public IBluetoothBroadcastService CreateBroadcastService(IBluetoothBroadcaster broadcaster,
        IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest request)
    {
        return new BluetoothBroadcastService(broadcaster, request, CharacteristicFactory);
    }
}
