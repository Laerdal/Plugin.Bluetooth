using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth broadcaster starts unexpectedly.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class BroadcasterUnexpectedStartException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterUnexpectedStartException"/> class.
    /// </summary>
    public BroadcasterUnexpectedStartException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterUnexpectedStartException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BroadcasterUnexpectedStartException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterUnexpectedStartException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BroadcasterUnexpectedStartException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterUnexpectedStartException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterUnexpectedStartException(IBluetoothBroadcaster broadcaster, string message = "Broadcaster started unexpectedly", Exception? innerException = null) : base(broadcaster, message, innerException)
    {
    }
}