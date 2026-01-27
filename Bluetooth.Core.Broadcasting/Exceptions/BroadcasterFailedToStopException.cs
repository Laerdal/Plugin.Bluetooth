using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth broadcaster fails to stop.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class BroadcasterFailedToStopException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterFailedToStopException"/> class.
    /// </summary>
    public BroadcasterFailedToStopException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterFailedToStopException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BroadcasterFailedToStopException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterFailedToStopException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BroadcasterFailedToStopException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterFailedToStopException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterFailedToStopException(IBluetoothBroadcaster broadcaster, string message = "Failed to stop broadcaster", Exception? innerException = null) : base(broadcaster, message, innerException)
    {
    }
}