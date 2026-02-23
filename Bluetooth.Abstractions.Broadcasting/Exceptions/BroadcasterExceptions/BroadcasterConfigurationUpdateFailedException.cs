namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth broadcaster configuration update fails.
/// </summary>
/// <seealso cref="IBluetoothBroadcaster" />
/// <seealso cref="BroadcasterException" />
public class BroadcasterConfigurationUpdateFailedException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterConfigurationUpdateFailedException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterConfigurationUpdateFailedException(IBluetoothBroadcaster broadcaster, string message = "Failed to update broadcaster configuration", Exception? innerException = null) : base(broadcaster, message, innerException)
    {
    }
}
