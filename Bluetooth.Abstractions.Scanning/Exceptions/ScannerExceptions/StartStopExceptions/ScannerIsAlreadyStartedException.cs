namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner is already started.
/// </summary>
/// <seealso cref="ScannerFailedToStartException" />
public class ScannerIsAlreadyStartedException : ScannerFailedToStartException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStartedException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerIsAlreadyStartedException(
        IBluetoothScanner scanner,
        string message = "Scanner is already started",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}