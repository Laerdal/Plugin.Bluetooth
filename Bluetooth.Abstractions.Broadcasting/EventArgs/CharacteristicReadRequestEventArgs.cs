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
    /// <param name="currentValue">The current value of the characteristic, used as the default response.</param>
    public CharacteristicReadRequestEventArgs(string clientId, Guid serviceId, Guid characteristicId, ReadOnlyMemory<byte> currentValue)
    {
        ClientId = clientId;
        ServiceId = serviceId;
        CharacteristicId = characteristicId;
        ResponseValue = currentValue;
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

    /// <summary>
    ///     Gets or sets the value to return to the client.
    ///     Defaults to the characteristic's current value. Set this property to override the response.
    /// </summary>
    public ReadOnlyMemory<byte> ResponseValue { get; set; }
}
