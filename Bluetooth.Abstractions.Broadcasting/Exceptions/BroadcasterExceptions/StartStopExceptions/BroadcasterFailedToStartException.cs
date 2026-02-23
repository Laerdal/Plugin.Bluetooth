namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth broadcaster fails to start.
/// </summary>
/// <seealso cref="IBluetoothBroadcaster" />
/// <seealso cref="BroadcasterException" />
public class BroadcasterFailedToStartException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterFailedToStartException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterFailedToStartException(IBluetoothBroadcaster broadcaster, string message = "Failed to start broadcaster", Exception? innerException = null) : base(broadcaster, message, innerException)
    {
    }
}
