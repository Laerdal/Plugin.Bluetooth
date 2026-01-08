namespace Bluetooth.Core.EventArgs;

/// <summary>
/// Event arguments for when a client requests to read a characteristic value.
/// </summary>
public class CharacteristicReadRequestEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicReadRequestEventArgs" /> class.
    /// </summary>
    /// <param name="clientId">The unique identifier of the requesting client.</param>
    /// <param name="serviceId">The service identifier containing the characteristic.</param>
    /// <param name="characteristicId">The characteristic identifier being read.</param>
    /// <param name="offset">The offset from which to start reading.</param>
    public CharacteristicReadRequestEventArgs(string clientId, Guid serviceId, Guid characteristicId, int offset = 0)
    {
        ClientId = clientId;
        ServiceId = serviceId;
        CharacteristicId = characteristicId;
        Offset = offset;
    }

    /// <summary>
    /// Gets the unique identifier of the requesting client.
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// Gets the service identifier containing the characteristic.
    /// </summary>
    public Guid ServiceId { get; }

    /// <summary>
    /// Gets the characteristic identifier being read.
    /// </summary>
    public Guid CharacteristicId { get; }

    /// <summary>
    /// Gets the offset from which to start reading.
    /// </summary>
    public int Offset { get; }

    /// <summary>
    /// Gets or sets the value to be sent to the client in response to the read request.
    /// Set this property in the event handler to provide the requested value.
    /// </summary>
    public byte[]? Value { get; set; }
}
