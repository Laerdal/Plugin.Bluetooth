using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to access or remove a service that does not exist in a Bluetooth broadcaster.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class BroadcasterServiceNotFoundException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterServiceNotFoundException"/> class.
    /// </summary>
    public BroadcasterServiceNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterServiceNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BroadcasterServiceNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterServiceNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BroadcasterServiceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterServiceNotFoundException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="serviceId">The UUID of the service that was not found.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterServiceNotFoundException(IBluetoothBroadcaster broadcaster, Guid serviceId, string? message = null, Exception? innerException = null) 
        : base(broadcaster, message ?? $"Service with ID '{serviceId}' does not exist in the broadcaster", innerException)
    {
        ServiceId = serviceId;
    }

    /// <summary>
    ///     Gets the UUID of the service that was not found.
    /// </summary>
    public Guid ServiceId { get; }
}