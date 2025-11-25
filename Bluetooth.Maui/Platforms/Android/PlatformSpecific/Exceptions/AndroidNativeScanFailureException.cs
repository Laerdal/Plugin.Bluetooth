using Exception = System.Exception;

namespace Bluetooth.Maui.PlatformSpecific.Exceptions;

/// <summary>
/// Represents an exception that occurs when Android Bluetooth LE scanning fails.
/// </summary>
/// <remarks>
/// This exception wraps Android's ScanFailure enum values to provide detailed
/// information about why Bluetooth LE scanning operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeScanFailureException : AndroidNativeBluetoothException
{
    /// <summary>
    /// Gets the specific ScanFailure that caused this exception.
    /// </summary>
    public ScanFailure ScanFailure { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeScanFailureException"/> class.
    /// </summary>
    public AndroidNativeScanFailureException()
    {
        ScanFailure = (ScanFailure)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeScanFailureException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AndroidNativeScanFailureException(string message) : base(message)
    {
        ScanFailure = (ScanFailure)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeScanFailureException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AndroidNativeScanFailureException(string message, Exception innerException) : base(message, innerException)
    {
        ScanFailure = (ScanFailure)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeScanFailureException"/> class with the specified ScanFailure status.
    /// </summary>
    /// <param name="status">The ScanFailure status that caused this exception.</param>
    public AndroidNativeScanFailureException(ScanFailure status)
        : base($"Native ScanFailure Exception: {status} ({(int)status}) (0x{status:X})")
    {
        ScanFailure = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeScanFailureException"/> class with the specified ScanFailure status and inner exception.
    /// </summary>
    /// <param name="status">The ScanFailure status that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeScanFailureException(ScanFailure status, Exception innerException)
        : base($"Native ScanFailure Exception: {status} ({(int)status}) (0x{status:X})", innerException)
    {
        ScanFailure = status;
    }
}
