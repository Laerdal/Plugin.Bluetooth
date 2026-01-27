using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to add a service that already exists to a Bluetooth broadcaster.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class BroadcasterServiceAlreadyExistsException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterServiceAlreadyExistsException"/> class.
    /// </summary>
    public BroadcasterServiceAlreadyExistsException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterServiceAlreadyExistsException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BroadcasterServiceAlreadyExistsException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterServiceAlreadyExistsException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BroadcasterServiceAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterServiceAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="serviceId">The UUID of the service that already exists.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterServiceAlreadyExistsException(IBluetoothBroadcaster broadcaster, Guid serviceId, string? message = null, Exception? innerException = null) 
        : base(broadcaster, message ?? $"Service with ID '{serviceId}' already exists in the broadcaster", innerException)
    {
        ServiceId = serviceId;
    }

    /// <summary>
    ///     Gets the UUID of the service that already exists.
    /// </summary>
    public Guid ServiceId { get; }
}