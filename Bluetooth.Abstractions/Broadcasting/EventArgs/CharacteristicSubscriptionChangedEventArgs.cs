namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
/// Event arguments for when a client subscribes or unsubscribes from characteristic notifications/indications.
/// </summary>
public class CharacteristicSubscriptionChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicSubscriptionChangedEventArgs" /> class.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="serviceId">The service identifier containing the characteristic.</param>
    /// <param name="characteristicId">The characteristic identifier.</param>
    /// <param name="isSubscribed">Indicates whether the client is now subscribed.</param>
    /// <param name="isNotification">Indicates whether the subscription is for notifications (true) or indications (false).</param>
    public CharacteristicSubscriptionChangedEventArgs(string clientId, Guid serviceId, Guid characteristicId, bool isSubscribed, bool isNotification = true)
    {
        ClientId = clientId;
        ServiceId = serviceId;
        CharacteristicId = characteristicId;
        IsSubscribed = isSubscribed;
        IsNotification = isNotification;
    }

    /// <summary>
    /// Gets the unique identifier of the client.
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// Gets the service identifier containing the characteristic.
    /// </summary>
    public Guid ServiceId { get; }

    /// <summary>
    /// Gets the characteristic identifier.
    /// </summary>
    public Guid CharacteristicId { get; }

    /// <summary>
    /// Gets a value indicating whether the client is now subscribed.
    /// </summary>
    public bool IsSubscribed { get; }

    /// <summary>
    /// Gets a value indicating whether the subscription is for notifications (true) or indications (false).
    /// </summary>
    public bool IsNotification { get; }
}
