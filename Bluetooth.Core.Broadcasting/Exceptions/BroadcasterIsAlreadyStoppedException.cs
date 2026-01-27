using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth broadcaster is already stopped.
/// </summary>
/// <seealso cref="BroadcasterFailedToStopException" />
public class BroadcasterIsAlreadyStoppedException : BroadcasterFailedToStopException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterIsAlreadyStoppedException"/> class.
    /// </summary>
    public BroadcasterIsAlreadyStoppedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterIsAlreadyStoppedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BroadcasterIsAlreadyStoppedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterIsAlreadyStoppedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BroadcasterIsAlreadyStoppedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterIsAlreadyStoppedException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterIsAlreadyStoppedException(IBluetoothBroadcaster broadcaster, string message = "Broadcaster is already stopped", Exception? innerException = null) : base(broadcaster, message, innerException)
    {
    }

    /// <summary>
    ///     Throws an <see cref="BroadcasterIsAlreadyStoppedException"/> if the Bluetooth broadcaster is already stopped.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="broadcaster"/> is null.</exception>
    /// <exception cref="BroadcasterIsAlreadyStoppedException">Thrown when the broadcaster is already stopped.</exception>
    public static void ThrowIfIsStopped(IBluetoothBroadcaster broadcaster)
    {
        ArgumentNullException.ThrowIfNull(broadcaster);
        if (!broadcaster.IsRunning)
        {
            throw new BroadcasterIsAlreadyStoppedException(broadcaster);
        }
    }
}