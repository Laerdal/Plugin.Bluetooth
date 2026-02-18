namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner stops unexpectedly.
/// </summary>
/// <seealso cref="ScannerException" />
public class ScannerUnexpectedStopException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerUnexpectedStopException"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerUnexpectedStopException(
        IBluetoothScanner scanner,
        string message = "Scanner stopped unexpectedly",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}
