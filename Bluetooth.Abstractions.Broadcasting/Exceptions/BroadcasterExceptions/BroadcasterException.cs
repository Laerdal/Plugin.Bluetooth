namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth broadcaster operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth broadcaster associated with the error,
///     allowing for easier debugging and tracking of broadcaster-related issues.
/// </remarks>
public abstract class BroadcasterException : BluetoothBroadcastingException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected BroadcasterException(IBluetoothBroadcaster broadcaster, string message = "Unknown broadcaster exception", Exception? innerException = null) : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(broadcaster);
        Broadcaster = broadcaster;
    }

    /// <summary>
    ///     Gets the Bluetooth broadcaster associated with the exception.
    /// </summary>
    public IBluetoothBroadcaster Broadcaster { get; }
}
