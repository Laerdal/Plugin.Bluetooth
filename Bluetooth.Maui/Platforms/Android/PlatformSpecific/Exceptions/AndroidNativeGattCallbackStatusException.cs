namespace Bluetooth.Maui.PlatformSpecific.Exceptions;

/// <summary>
/// Represents an exception that occurs when Android GATT callback operations return a non-success status.
/// </summary>
/// <remarks>
/// This exception wraps Android's GattCallbackStatus enum values to provide detailed
/// information about why GATT callback operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeGattCallbackStatusException : AndroidNativeBluetoothException
{
    /// <summary>
    /// Gets the specific GattCallbackStatus that caused this exception.
    /// </summary>
    public GattCallbackStatus GattCallbackStatus { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusException"/> class.
    /// </summary>
    public AndroidNativeGattCallbackStatusException()
    {
        GattCallbackStatus = (GattCallbackStatus)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AndroidNativeGattCallbackStatusException(string message) : base(message)
    {
        GattCallbackStatus = (GattCallbackStatus)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AndroidNativeGattCallbackStatusException(string message, Exception innerException) : base(message, innerException)
    {
        GattCallbackStatus = (GattCallbackStatus)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusException"/> class with the specified GattCallbackStatus.
    /// </summary>
    /// <param name="status">The GattCallbackStatus that caused this exception.</param>
    public AndroidNativeGattCallbackStatusException(GattCallbackStatus status)
        : base($"Native GattCallbackStatus Exception: {status} ({(int)status}) (0x{status:X})")
    {
        GattCallbackStatus = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusException"/> class with the specified GattCallbackStatus and inner exception.
    /// </summary>
    /// <param name="status">The GattCallbackStatus that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeGattCallbackStatusException(GattCallbackStatus status, Exception innerException)
        : base($"Native GattCallbackStatus Exception: {status} ({(int)status}) (0x{status:X})", innerException)
    {
        GattCallbackStatus = status;
    }

    /// <summary>
    /// Throws an <see cref="AndroidNativeGattCallbackStatusException"/> if the status is not GATT_SUCCESS.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <exception cref="AndroidNativeGattCallbackStatusException">Thrown when the status is not GATT_SUCCESS.</exception>
    public static void ThrowIfNotSuccess(GattCallbackStatus status)
    {
        if (status != GattCallbackStatus.GATT_SUCCESS)
        {
            throw new AndroidNativeGattCallbackStatusException(status);
        }
    }

    /// <summary>
    /// Throws an <see cref="AndroidNativeGattCallbackStatusException"/> if the GattStatus is not Success.
    /// </summary>
    /// <param name="status">The GattStatus to check.</param>
    /// <exception cref="AndroidNativeGattCallbackStatusException">Thrown when the status is not Success.</exception>
    public static void ThrowIfNotSuccess(GattStatus status)
    {
        if ((GattCallbackStatus)status != GattCallbackStatus.GATT_SUCCESS)
        {
            throw new AndroidNativeGattCallbackStatusException((GattCallbackStatus)status);
        }
    }
}
