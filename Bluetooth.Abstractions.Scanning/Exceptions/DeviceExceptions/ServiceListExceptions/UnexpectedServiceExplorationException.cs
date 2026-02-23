namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected service exploration happens.
/// </summary>
/// <seealso cref="ServiceExplorationException" />
/// <seealso cref="DeviceException" />
public class UnexpectedServiceExplorationException : ServiceExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedServiceExplorationException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public UnexpectedServiceExplorationException(
        IBluetoothRemoteDevice device,
        string? message = null,
        Exception? innerException = null)
        : base(device, message ?? "Unexpected service exploration", innerException)
    {
    }
}
