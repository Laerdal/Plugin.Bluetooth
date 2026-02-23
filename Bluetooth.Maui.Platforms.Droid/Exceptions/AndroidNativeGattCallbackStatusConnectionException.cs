using Bluetooth.Maui.Platforms.Droid.Enums;

namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

/// <summary>
///     Represents an exception that occurs when Android GATT connection callback operations return a non-success status.
/// </summary>
/// <remarks>
///     This exception wraps Android's GattCallbackStatusConnection enum values to provide detailed
///     information about why GATT connection callback operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeGattCallbackStatusConnectionException : AndroidNativeBluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusConnectionException" /> class with the specified GattCallbackStatusConnection and inner exception.
    /// </summary>
    /// <param name="status">The GattCallbackStatusConnection that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeGattCallbackStatusConnectionException(GattCallbackStatusConnection status, Exception? innerException = null) : base($"{GattCallbackStatusConnectionToDescription(status)} : {status}", innerException)
    {
        GattCallbackStatusConnection = status;
    }

    /// <summary>
    ///     Gets the specific GattCallbackStatusConnection that caused this exception.
    /// </summary>
    public GattCallbackStatusConnection GattCallbackStatusConnection { get; }

    /// <summary>
    ///     Throws an <see cref="AndroidNativeGattCallbackStatusConnectionException" /> if the status is not GATT_SUCCESS.
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

    private static string GattCallbackStatusConnectionToDescription(GattCallbackStatusConnection status)
    {
        var statusCodeValue = (int) status;
        return statusCodeValue switch
        {
            0x00 => "Success: The GATT connection operation completed successfully.",
            0x01 => "Error: GATT connection L2CAP failure.",
            0x08 => "Error: GATT connection timeout.",
            0x13 => "Error: GATT connection terminated by peer user.",
            0x16 => "Error: GATT connection terminated by local host.",
            0x22 => "Error: GATT connection LMP timeout.",
            0x3E => "Error: GATT connection failed to establish.",
            0x85 => "Error: Generic GATT error.",
            0x0100 => "Error: GATT connection cancelled.",
            0x0101 => "Error: Too many open GATT connections.",
            _ => "Unknown GATT connection status code."
        };
    }
}
