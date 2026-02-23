namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner starts unexpectedly.
/// </summary>
/// <seealso cref="ScannerException" />
public class ScannerUnexpectedStartException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerUnexpectedStartException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerUnexpectedStartException(
        IBluetoothScanner scanner,
        string message = "Scanner started unexpectedly",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}
