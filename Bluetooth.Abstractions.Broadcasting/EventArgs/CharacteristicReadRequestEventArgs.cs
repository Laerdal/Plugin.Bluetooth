namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
///     Event arguments for when a client requests to read a characteristic value.
/// </summary>
public class CharacteristicReadRequestEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicReadRequestEventArgs" /> class.
    /// </summary>
    /// <param name="clientId">The unique identifier of the requesting client.</param>
    /// <param name="serviceId">The service identifier containing the characteristic.</param>
    /// <param name="characteristicId">The characteristic identifier being read.</param>
    public CharacteristicReadRequestEventArgs(string clientId, Guid serviceId, Guid characteristicId)
    {
        ClientId = clientId;
        ServiceId = serviceId;
        CharacteristicId = characteristicId;
    }

    /// <summary>
    ///     Gets the unique identifier of the requesting client.
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    ///     Gets the service identifier containing the characteristic.
    /// </summary>
    public Guid ServiceId { get; }

    /// <summary>
    ///     Gets the characteristic identifier being read.
    /// </summary>
    public Guid CharacteristicId { get; }
}