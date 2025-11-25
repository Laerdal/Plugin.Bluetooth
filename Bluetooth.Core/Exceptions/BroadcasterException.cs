namespace Bluetooth.Core.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth broadcaster operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth broadcaster associated with the error,
///     allowing for easier debugging and tracking of broadcaster-related issues.
/// </remarks>
/// <seealso cref="IBluetoothBroadcaster" />
/// <seealso cref="ActivityException" />
public abstract class BroadcasterException : ActivityException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterException"/> class.
    /// </summary>
    protected BroadcasterException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected BroadcasterException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected BroadcasterException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected BroadcasterException(
        IBluetoothBroadcaster broadcaster,
        string message = "Unknown broadcaster exception",
        Exception? innerException = null)
        : base(broadcaster, message, innerException)
    {
    }
}
