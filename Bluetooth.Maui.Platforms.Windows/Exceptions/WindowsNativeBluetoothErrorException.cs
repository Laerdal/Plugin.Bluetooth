namespace Bluetooth.Maui.Platforms.Windows.Exceptions;

/// <summary>
/// Represents an exception that occurs when Windows Bluetooth operations return a non-success error.
/// </summary>
/// <remarks>
/// This exception wraps Windows' BluetoothError enum values to provide detailed
/// information about why Bluetooth operations failed.
/// </remarks>
/// <seealso cref="WindowsNativeBluetoothException" />
public class WindowsNativeBluetoothErrorException : WindowsNativeBluetoothException
{
    /// <summary>
    /// Gets the specific BluetoothError that caused this exception.
    /// </summary>
    public BluetoothError BluetoothError { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothErrorException"/> class with the specified BluetoothError and inner exception.
    /// </summary>
    /// <param name="error">The BluetoothError that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public WindowsNativeBluetoothErrorException(BluetoothError error, Exception? innerException = null)
        : base($"{BluetoothErrorToDescription(error)} : {error} ({(int)error})", innerException)
    {
        BluetoothError = error;
    }

    /// <summary>
    /// Throws a <see cref="WindowsNativeBluetoothErrorException"/> if the error is not Success.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <exception cref="WindowsNativeBluetoothErrorException">Thrown when the error is not Success.</exception>
    public static void ThrowIfNotSuccess(BluetoothError error)
    {
        if (error != BluetoothError.Success)
        {
            throw new WindowsNativeBluetoothErrorException(error);
        }
    }

    private static string BluetoothErrorToDescription(BluetoothError error)
    {
        var errorCodeValue = (int)error;
        return errorCodeValue switch
        {
            0 => "Success: The operation was successfully completed or serviced.",
            1 => "Error: The Bluetooth radio was not available. This error occurs when the Bluetooth radio has been turned off.",
            2 => "Error: The operation cannot be serviced because the necessary resources are currently in use.",
            3 => "Error: The operation cannot be completed because the remote device is not connected.",
            4 => "Error: An unexpected error has occurred.",
            5 => "Error: The operation is disabled by policy.",
            6 => "Error: The operation is not supported on the current Bluetooth radio hardware.",
            7 => "Error: The operation is disabled by the user.",
            8 => "Error: The operation requires consent.",
            9 => "Error: The transport is not supported.",
            _ => "Unknown Bluetooth error."
        };
    }
}

