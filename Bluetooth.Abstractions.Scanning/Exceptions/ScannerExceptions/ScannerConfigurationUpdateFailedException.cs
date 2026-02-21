namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner configuration update fails.
/// </summary>
/// <seealso cref="ScannerException" />
public class ScannerConfigurationUpdateFailedException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerConfigurationUpdateFailedException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerConfigurationUpdateFailedException(
        IBluetoothScanner scanner,
        string message = "Failed to update scanner configuration",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}