namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected service exploration happens.
/// </summary>
/// <seealso cref="DeviceExplorationException" />
/// <seealso cref="ScannerException" />
public class UnexpectedDeviceExplorationException : DeviceExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedDeviceExplorationException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public UnexpectedDeviceExplorationException(
        IBluetoothScanner scanner,
        string? message = null,
        Exception? innerException = null)
        : base(scanner, message ?? "Unexpected device exploration", innerException)
    {
    }
}
