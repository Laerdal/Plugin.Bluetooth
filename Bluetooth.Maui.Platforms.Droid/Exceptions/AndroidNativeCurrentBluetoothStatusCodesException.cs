
namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

/// <summary>
/// Represents an exception that occurs when Android Bluetooth operations return a non-success status code.
/// </summary>
/// <remarks>
/// This exception wraps Android's CurrentBluetoothStatusCodes enum values to provide detailed
/// information about why Bluetooth operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeCurrentBluetoothStatusCodesException : AndroidNativeBluetoothException
{
    /// <summary>
    /// Gets the specific CurrentBluetoothStatusCodes that caused this exception.
    /// </summary>
    public CurrentBluetoothStatusCodes CurrentBluetoothStatusCodes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeCurrentBluetoothStatusCodesException"/> class.
    /// </summary>
    public AndroidNativeCurrentBluetoothStatusCodesException()
    {
        CurrentBluetoothStatusCodes = (CurrentBluetoothStatusCodes)(-1); // Default to error state
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeCurrentBluetoothStatusCodesException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AndroidNativeCurrentBluetoothStatusCodesException(string message) : base(message)
    {
        CurrentBluetoothStatusCodes = (CurrentBluetoothStatusCodes)(-1); // Default to error state
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeCurrentBluetoothStatusCodesException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AndroidNativeCurrentBluetoothStatusCodesException(string message, Exception innerException) : base(message, innerException)
    {
        CurrentBluetoothStatusCodes = (CurrentBluetoothStatusCodes)(-1); // Default to error state
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeCurrentBluetoothStatusCodesException"/> class with the specified CurrentBluetoothStatusCodes status.
    /// </summary>
    /// <param name="status">The CurrentBluetoothStatusCodes status that caused this exception.</param>
    public AndroidNativeCurrentBluetoothStatusCodesException(CurrentBluetoothStatusCodes status)
        : base($"Native CurrentBluetoothStatusCodes Exception: {status} ({(int)status}) (0x{status:X})")
    {
        CurrentBluetoothStatusCodes = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeCurrentBluetoothStatusCodesException"/> class with the specified CurrentBluetoothStatusCodes status and inner exception.
    /// </summary>
    /// <param name="status">The CurrentBluetoothStatusCodes status that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeCurrentBluetoothStatusCodesException(CurrentBluetoothStatusCodes status, Exception innerException)
        : base($"Native CurrentBluetoothStatusCodes Exception: {status} ({(int)status}) (0x{status:X})", innerException)
    {
        CurrentBluetoothStatusCodes = status;
    }

    /// <summary>
    /// Throws an <see cref="AndroidNativeCurrentBluetoothStatusCodesException"/> if the status is not Success.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <exception cref="AndroidNativeCurrentBluetoothStatusCodesException">Thrown when the status is not Success.</exception>
    public static void ThrowIfNotSuccess(CurrentBluetoothStatusCodes status)
    {
        if (status != CurrentBluetoothStatusCodes.Success)
        {
            throw new AndroidNativeCurrentBluetoothStatusCodesException(status);
        }
    }
}
