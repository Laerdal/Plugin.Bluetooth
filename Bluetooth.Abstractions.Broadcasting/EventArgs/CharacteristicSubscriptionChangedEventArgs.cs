namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
///     Event arguments for when a client device subscribes to or unsubscribes from a characteristic.
/// </summary>
public class CharacteristicSubscriptionChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicSubscriptionChangedEventArgs" /> class.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client device.</param>
    /// <param name="serviceId">The service identifier containing the characteristic.</param>
    /// <param name="characteristicId">The characteristic identifier the client subscribed to or unsubscribed from.</param>
    public CharacteristicSubscriptionChangedEventArgs(string clientId, Guid serviceId, Guid characteristicId)
    {
        ClientId = clientId;
        ServiceId = serviceId;
        CharacteristicId = characteristicId;
    }

    /// <summary>
    ///     Gets the unique identifier of the client device.
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    ///     Gets the service identifier containing the characteristic.
    /// </summary>
    public Guid ServiceId { get; }

    /// <summary>
    ///     Gets the characteristic identifier the client subscribed to or unsubscribed from.
    /// </summary>
    public Guid CharacteristicId { get; }
}
