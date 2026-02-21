namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

/// <summary>
///     Represents an exception that occurs in Android-specific native Bluetooth operations.
/// </summary>
/// <remarks>
///     This exception is used for wrapping Android-specific Bluetooth exceptions
///     and providing a unified exception model for Android platform operations.
/// </remarks>
/// <seealso cref="BluetoothException" />
public class AndroidNativeBluetoothException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidNativeBluetoothException" /> class.
    /// </summary>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public AndroidNativeBluetoothException(string? message = null, Exception? innerException = null)
        : base(message ?? "An error occurred in Android native Bluetooth operations", innerException)
    {
    }
}