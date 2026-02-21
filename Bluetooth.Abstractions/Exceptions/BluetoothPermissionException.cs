namespace Bluetooth.Abstractions.Exceptions;

/// <summary>
///     Exception thrown when Bluetooth permission requests fail or are denied.
///     Wraps platform-specific exceptions (COMException, SecurityException, etc.) to provide a unified API surface.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="innerException">The platform-specific exception that is the cause of the current exception, or null if no inner exception is specified.</param>
public class BluetoothPermissionException(string message, Exception? innerException = null)
    : BluetoothException(message, innerException)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothPermissionException" /> class with a default message.
    /// </summary>
    public BluetoothPermissionException() : this("Bluetooth permission request failed or was denied")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothPermissionException" /> class with the specified message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public BluetoothPermissionException(string message) : this(message, null)
    {
    }
}
