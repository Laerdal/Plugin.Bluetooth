namespace Bluetooth.Abstractions.Exceptions;

/// <summary>
///     The base class for all Bluetooth-related exceptions in the Bluetooth.Core library.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
public abstract class BluetoothException(string message, Exception? innerException) : Exception(message, innerException)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothException" /> class with a default message.
    /// </summary>
    protected BluetoothException() : this("Unknown bluetooth exception", null)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothException" /> class with the specified message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    protected BluetoothException(string message) : this(message, null)
    {
    }
}
