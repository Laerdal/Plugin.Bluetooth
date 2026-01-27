namespace Bluetooth.Maui.PlatformSpecific.Exceptions;

/// <summary>
/// Represents an exception that occurs when Android GATT connection callback operations return a non-success status.
/// </summary>
/// <remarks>
/// This exception wraps Android's GattCallbackStatusConnection enum values to provide detailed
/// information about why GATT connection callback operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeGattCallbackStatusConnectionException : AndroidNativeBluetoothException
{
    /// <summary>
    /// Gets the specific GattCallbackStatusConnection that caused this exception.
    /// </summary>
    public GattCallbackStatusConnection GattCallbackStatusConnection { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusConnectionException"/> class.
    /// </summary>
    public AndroidNativeGattCallbackStatusConnectionException()
    {
        GattCallbackStatusConnection = (GattCallbackStatusConnection)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusConnectionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AndroidNativeGattCallbackStatusConnectionException(string message) : base(message)
    {
        GattCallbackStatusConnection = (GattCallbackStatusConnection)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusConnectionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AndroidNativeGattCallbackStatusConnectionException(string message, Exception innerException) : base(message, innerException)
    {
        GattCallbackStatusConnection = (GattCallbackStatusConnection)1; // Default to first failure type
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusConnectionException"/> class with the specified GattCallbackStatusConnection.
    /// </summary>
    /// <param name="status">The GattCallbackStatusConnection that caused this exception.</param>
    public AndroidNativeGattCallbackStatusConnectionException(GattCallbackStatusConnection status)
        : base($"Native GattCallbackStatusConnection Exception: {status} ({(int)status}) (0x{status:X})")
    {
        GattCallbackStatusConnection = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusConnectionException"/> class with the specified GattCallbackStatusConnection and inner exception.
    /// </summary>
    /// <param name="status">The GattCallbackStatusConnection that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeGattCallbackStatusConnectionException(GattCallbackStatusConnection status, Exception innerException)
        : base($"Native GattCallbackStatusConnection Exception: {status} ({(int)status}) (0x{status:X})", innerException)
    {
        GattCallbackStatusConnection = status;
    }

    /// <summary>
    /// Throws an <see cref="AndroidNativeGattCallbackStatusConnectionException"/> if the status is not GATT_SUCCESS.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <exception cref="AndroidNativeGattCallbackStatusConnectionException">Thrown when the status is not GATT_SUCCESS.</exception>
    public static void ThrowIfNotSuccess(GattCallbackStatusConnection status)
    {
        if (status != GattCallbackStatusConnection.GATT_SUCCESS)
        {
            throw new AndroidNativeGattCallbackStatusConnectionException(status);
        }
    }

    /// <summary>
    /// Throws an <see cref="AndroidNativeGattCallbackStatusConnectionException"/> if the GattStatus is not Success.
    /// </summary>
    /// <param name="status">The GattStatus to check.</param>
    /// <exception cref="AndroidNativeGattCallbackStatusConnectionException">Thrown when the status is not Success.</exception>
    public static void ThrowIfNotSuccess(GattStatus status)
    {
        if ((GattCallbackStatusConnection)status != GattCallbackStatusConnection.GATT_SUCCESS)
        {
            throw new AndroidNativeGattCallbackStatusConnectionException((GattCallbackStatusConnection)status);
        }
    }
}
