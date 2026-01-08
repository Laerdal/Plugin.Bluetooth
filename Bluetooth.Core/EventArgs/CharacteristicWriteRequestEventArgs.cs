namespace Bluetooth.Core.EventArgs;

/// <summary>
/// Event arguments for when a client requests to write a value to a characteristic.
/// </summary>
public class CharacteristicWriteRequestEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicWriteRequestEventArgs" /> class.
    /// </summary>
    /// <param name="clientId">The unique identifier of the requesting client.</param>
    /// <param name="serviceId">The service identifier containing the characteristic.</param>
    /// <param name="characteristicId">The characteristic identifier being written to.</param>
    /// <param name="value">The value being written.</param>
    /// <param name="offset">The offset at which to start writing.</param>
    /// <param name="withResponse">Indicates whether the write requires a response.</param>
    public CharacteristicWriteRequestEventArgs(string clientId, Guid serviceId, Guid characteristicId, byte[] value, int offset = 0, bool withResponse = true)
    {
        ClientId = clientId;
        ServiceId = serviceId;
        CharacteristicId = characteristicId;
        Value = value;
        Offset = offset;
        WithResponse = withResponse;
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
    /// Gets the characteristic identifier being written to.
    /// </summary>
    public Guid CharacteristicId { get; }

    /// <summary>
    /// Gets the value being written.
    /// </summary>
    public byte[] Value { get; }

    /// <summary>
    /// Gets the offset at which to start writing.
    /// </summary>
    public int Offset { get; }

    /// <summary>
    /// Gets a value indicating whether the write requires a response.
    /// </summary>
    public bool WithResponse { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the write request should be accepted.
    /// Set this property in the event handler to accept or reject the write operation.
    /// </summary>
    public bool Accept { get; set; } = true;
}
