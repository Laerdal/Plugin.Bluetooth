namespace Bluetooth.Maui.PlatformSpecific.Exceptions;

/// <summary>
/// Represents an exception that occurs in Android-specific native Bluetooth operations.
/// </summary>
/// <remarks>
/// This exception is used for wrapping Android-specific Bluetooth exceptions
/// and providing a unified exception model for Android platform operations.
/// </remarks>
/// <seealso cref="BluetoothException" />
public class AndroidNativeBluetoothException : BluetoothException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeBluetoothException"/> class.
    /// </summary>
    public AndroidNativeBluetoothException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeBluetoothException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AndroidNativeBluetoothException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeBluetoothException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AndroidNativeBluetoothException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
