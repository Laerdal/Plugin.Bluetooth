namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner fails to stop.
/// </summary>
/// <seealso cref="ScannerException" />
public class ScannerFailedToStopException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerFailedToStopException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerFailedToStopException(
        IBluetoothScanner scanner,
        string message = "Failed to stop scanner",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}