using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to add a characteristic that already exists to a Bluetooth broadcast service.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class BroadcasterCharacteristicAlreadyExistsException : BroadcastServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterCharacteristicAlreadyExistsException"/> class.
    /// </summary>
    public BroadcasterCharacteristicAlreadyExistsException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterCharacteristicAlreadyExistsException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BroadcasterCharacteristicAlreadyExistsException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterCharacteristicAlreadyExistsException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BroadcasterCharacteristicAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterCharacteristicAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth broadcast service associated with the exception.</param>
    /// <param name="characteristicId">The UUID of the characteristic that already exists.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterCharacteristicAlreadyExistsException(IBluetoothBroadcastService service, Guid characteristicId, string? message = null, Exception? innerException = null)
        : base(service, message ?? $"Characteristic with ID '{characteristicId}' already exists in service '{service?.Name}' ({service?.Id})", innerException)
    {
        CharacteristicId = characteristicId;
    }

    /// <summary>
    ///     Gets the UUID of the characteristic that already exists.
    /// </summary>
    public Guid CharacteristicId { get; }
}
