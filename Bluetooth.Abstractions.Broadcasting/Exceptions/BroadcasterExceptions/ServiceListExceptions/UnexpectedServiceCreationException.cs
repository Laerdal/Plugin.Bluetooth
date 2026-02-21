namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected Broadcast Service exploration happens.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class UnexpectedServiceCreationException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedServiceCreationException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public UnexpectedServiceCreationException(
        IBluetoothBroadcaster broadcaster,
        string? message = null,
        Exception? innerException = null)
        : base(broadcaster, message ?? "Unexpected broadcast service exploration", innerException)
    {
    }
}