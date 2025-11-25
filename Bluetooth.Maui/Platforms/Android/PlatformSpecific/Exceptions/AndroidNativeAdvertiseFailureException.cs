namespace Bluetooth.Maui.PlatformSpecific.Exceptions;

/// <summary>
/// Represents an exception that occurs when Android Bluetooth LE advertising fails.
/// </summary>
/// <remarks>
/// This exception wraps Android's AdvertiseFailure enum values to provide detailed
/// information about why Bluetooth LE advertising operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeAdvertiseFailureException : AndroidNativeBluetoothException
{
    /// <summary>
    /// Gets the specific AdvertiseFailure that caused this exception.
    /// </summary>
    public AdvertiseFailure AdvertiseFailure { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeAdvertiseFailureException"/> class.
    /// </summary>
    public AndroidNativeAdvertiseFailureException()
    {
        AdvertiseFailure = (AdvertiseFailure)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeAdvertiseFailureException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AndroidNativeAdvertiseFailureException(string message) : base(message)
    {
        AdvertiseFailure = (AdvertiseFailure)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeAdvertiseFailureException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AndroidNativeAdvertiseFailureException(string message, Exception innerException) : base(message, innerException)
    {
        AdvertiseFailure = (AdvertiseFailure)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeAdvertiseFailureException"/> class with the specified AdvertiseFailure status.
    /// </summary>
    /// <param name="status">The AdvertiseFailure status that caused this exception.</param>
    public AndroidNativeAdvertiseFailureException(AdvertiseFailure status)
        : base($"Native AdvertiseFailure Exception: {status} ({(int)status}) (0x{status:X})")
    {
        AdvertiseFailure = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeAdvertiseFailureException"/> class with the specified AdvertiseFailure status and inner exception.
    /// </summary>
    /// <param name="status">The AdvertiseFailure status that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeAdvertiseFailureException(AdvertiseFailure status, Exception innerException)
        : base($"Native AdvertiseFailure Exception: {status} ({(int)status}) (0x{status:X})", innerException)
    {
        AdvertiseFailure = status;
    }
}
