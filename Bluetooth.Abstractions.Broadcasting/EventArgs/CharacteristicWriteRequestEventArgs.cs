namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
///     Event arguments for when a client requests to write a value to a characteristic.
/// </summary>
public class CharacteristicWriteRequestEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicWriteRequestEventArgs" /> class.
    /// </summary>
    /// <param name="clientId">The unique identifier of the requesting client.</param>
    /// <param name="serviceId">The service identifier containing the characteristic.</param>
    /// <param name="characteristicId">The characteristic identifier being written to.</param>
    /// <param name="value">The value being written.</param>
    public CharacteristicWriteRequestEventArgs(string clientId, Guid serviceId, Guid characteristicId, ReadOnlyMemory<byte> value)
    {
        ClientId = clientId;
        ServiceId = serviceId;
        CharacteristicId = characteristicId;
        Value = value;
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
    ///     Gets the characteristic identifier being written to.
    /// </summary>
    public Guid CharacteristicId { get; }

    /// <summary>
    ///     Gets the value being written.
    /// </summary>
    public ReadOnlyMemory<byte> Value { get; }
}
