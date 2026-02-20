namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when device exploration fails.
/// </summary>
/// <seealso cref="ScannerException" />
public class DeviceExplorationException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceExplorationException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceExplorationException(
        IBluetoothScanner scanner,
        string? message = null,
        Exception? innerException = null)
        : base(scanner, message ?? "Unknown failure while exploring devices", innerException)
    {
    }
}