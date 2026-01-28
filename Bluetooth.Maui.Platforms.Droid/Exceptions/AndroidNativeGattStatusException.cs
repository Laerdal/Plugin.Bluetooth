using Exception = System.Exception;

namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

/// <summary>
/// Represents an exception that occurs when Android GATT operations return a non-success status.
/// </summary>
/// <remarks>
/// This exception wraps Android's GattStatus enum values to provide detailed
/// information about why GATT operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeGattStatusException : AndroidNativeBluetoothException
{
    /// <summary>
    /// Gets the specific GattStatus that caused this exception.
    /// </summary>
    public GattStatus GattStatus { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattStatusException"/> class.
    /// </summary>
    public AndroidNativeGattStatusException()
    {
        GattStatus = GattStatus.Failure;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattStatusException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AndroidNativeGattStatusException(string message) : base(message)
    {
        GattStatus = GattStatus.Failure;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattStatusException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AndroidNativeGattStatusException(string message, Exception innerException) : base(message, innerException)
    {
        GattStatus = GattStatus.Failure;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattStatusException"/> class with the specified GattStatus.
    /// </summary>
    /// <param name="status">The GattStatus that caused this exception.</param>
    public AndroidNativeGattStatusException(GattStatus status)
        : base($"Native GattStatus Exception: {status} ({(int)status}) (0x{status:X})")
    {
        GattStatus = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattStatusException"/> class with the specified GattStatus and inner exception.
    /// </summary>
    /// <param name="status">The GattStatus that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeGattStatusException(GattStatus status, Exception innerException)
        : base($"Native GattStatus Exception: {status} ({(int)status}) (0x{status:X})", innerException)
    {
        GattStatus = status;
    }

    /// <summary>
    /// Throws an <see cref="AndroidNativeGattStatusException"/> if the status is not Success.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <exception cref="AndroidNativeGattStatusException">Thrown when the status is not Success.</exception>
    public static void ThrowIfNotSuccess(GattStatus status)
    {
        if (status != GattStatus.Success)
        {
            throw new AndroidNativeGattStatusException(status);
        }
    }
}
