namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth broadcaster is already stopped.
/// </summary>
/// <seealso cref="IBluetoothBroadcaster" />
/// <seealso cref="BroadcasterException" />
/// <seealso cref="BroadcasterFailedToStopException" />
public class BroadcasterIsAlreadyStoppedException : BroadcasterFailedToStopException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterIsAlreadyStoppedException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterIsAlreadyStoppedException(IBluetoothBroadcaster broadcaster, string message = "Broadcaster is already stopped", Exception? innerException = null) : base(broadcaster, message, innerException)
    {
    }

    /// <summary>
    ///     Throws an <see cref="BroadcasterIsAlreadyStoppedException" /> if the Bluetooth broadcaster is already stopped.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="broadcaster" /> is null.</exception>
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