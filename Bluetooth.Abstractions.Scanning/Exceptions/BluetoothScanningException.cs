namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Exception thrown when an error occurs during Bluetooth scanning operations.
/// </summary>
public class BluetoothScanningException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothScanningException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BluetoothScanningException(string message = "Unknown Bluetooth scanning exception", Exception? innerException = null) : base(message, innerException)
    {
    }
}