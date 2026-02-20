namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner is already stopped.
/// </summary>
/// <seealso cref="ScannerFailedToStopException" />
public class ScannerIsAlreadyStoppedException : ScannerFailedToStopException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStoppedException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerIsAlreadyStoppedException(
        IBluetoothScanner scanner,
        string message = "Scanner is already stopped",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}