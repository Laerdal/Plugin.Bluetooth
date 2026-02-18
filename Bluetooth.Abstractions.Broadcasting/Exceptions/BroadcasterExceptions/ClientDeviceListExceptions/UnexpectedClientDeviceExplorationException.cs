namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected ClientDevice exploration happens.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class UnexpectedClientDeviceExplorationException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedClientDeviceExplorationException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public UnexpectedClientDeviceExplorationException(
        IBluetoothBroadcaster broadcaster,
        string? message = null,
        Exception? innerException = null)
        : base(broadcaster, message ?? "Unexpected client device exploration", innerException)
    {
    }
}
