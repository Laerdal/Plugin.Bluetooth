using Bluetooth.Abstractions.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

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
    public IBluetoothBroadcastService CreateService(IBluetoothBroadcaster broadcaster,
        IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest request)
    {
        return new BluetoothBroadcastService(broadcaster, request, CharacteristicFactory);
    }
}
