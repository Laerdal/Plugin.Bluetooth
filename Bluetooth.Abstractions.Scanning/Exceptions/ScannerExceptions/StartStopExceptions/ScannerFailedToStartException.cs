namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner fails to start.
/// </summary>
/// <seealso cref="ScannerException" />
public class ScannerFailedToStartException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerFailedToStartException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerFailedToStartException(
        IBluetoothScanner scanner,
        string message = "Failed to start scanner",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}
