namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth scanner operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth scanner associated with the error,
///     allowing for easier debugging and tracking of scanner-related issues.
/// </remarks>
/// <seealso cref="IBluetoothScanner" />
/// <seealso cref="ScannerException" />
public abstract class ScannerException : BluetoothScanningException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ScannerException(
        IBluetoothScanner scanner,
        string message = "Unknown scanner exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        Scanner = scanner;
    }

    /// <summary>
    ///     Gets the Bluetooth scanner associated with the exception.
    /// </summary>
    public IBluetoothScanner? Scanner { get; }
}