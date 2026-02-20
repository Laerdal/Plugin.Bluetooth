namespace Bluetooth.Maui.Platforms.Windows.Exceptions;

/// <summary>
///     Represents a Windows-specific native Bluetooth exception that provides detailed error descriptions
///     for various Windows Bluetooth API error conditions.
/// </summary>
public class WindowsNativeBluetoothException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsNativeBluetoothException" /> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public WindowsNativeBluetoothException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}