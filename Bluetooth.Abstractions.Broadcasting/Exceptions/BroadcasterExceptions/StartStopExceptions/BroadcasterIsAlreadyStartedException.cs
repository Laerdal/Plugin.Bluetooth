namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth broadcaster is already started.
/// </summary>
/// <seealso cref="IBluetoothBroadcaster" />
/// <seealso cref="BroadcasterException" />
/// <seealso cref="BroadcasterFailedToStartException" />
public class BroadcasterIsAlreadyStartedException : BroadcasterFailedToStartException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterIsAlreadyStartedException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterIsAlreadyStartedException(IBluetoothBroadcaster broadcaster, string message = "Broadcaster is already started", Exception? innerException = null) : base(broadcaster, message, innerException)
    {
    }

    /// <summary>
    ///     Throws an <see cref="BroadcasterIsAlreadyStartedException"/> if the Bluetooth broadcaster is already started.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="broadcaster"/> is null.</exception>
    /// <exception cref="BroadcasterIsAlreadyStartedException">Thrown when the broadcaster is already running.</exception>
    public static void ThrowIfIsStarted(IBluetoothBroadcaster broadcaster)
    {
        ArgumentNullException.ThrowIfNull(broadcaster);
        if (broadcaster.IsRunning)
        {
            throw new BroadcasterIsAlreadyStartedException(broadcaster);
        }
    }
}
