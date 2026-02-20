namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Exception thrown when an error occurs during Bluetooth broadcasting operations.
/// </summary>
public class BluetoothBroadcastingException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothBroadcastingException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BluetoothBroadcastingException(string message = "Unknown Bluetooth broadcasting exception", Exception? innerException = null) : base(message, innerException)
    {
    }
}