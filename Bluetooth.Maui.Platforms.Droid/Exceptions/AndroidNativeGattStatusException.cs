using Exception = System.Exception;

namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

/// <summary>
///     Represents an exception that occurs when Android GATT operations return a non-success status.
/// </summary>
/// <remarks>
///     This exception wraps Android's GattStatus enum values to provide detailed
///     information about why GATT operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeGattStatusException : AndroidNativeBluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidNativeGattStatusException" /> class with the specified GattStatus and inner exception.
    /// </summary>
    /// <param name="status">The GattStatus that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeGattStatusException(GattStatus status, Exception? innerException = null)
        : base($"{GattStatusToDescription(status)} : {status}", innerException)
    {
        GattStatus = status;
    }

    /// <summary>
    ///     Gets the specific GattStatus that caused this exception.
    /// </summary>
    public GattStatus GattStatus { get; }

    /// <summary>
    ///     Throws an <see cref="AndroidNativeGattStatusException" /> if the status is not Success.
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

    private static string GattStatusToDescription(GattStatus status)
    {
        var statusCodeValue = (int) status;
        return statusCodeValue switch
        {
            0 => "Success: The GATT operation completed successfully.",
            1 => "Error: Invalid GATT handle.",
            2 => "Error: Read operation is not permitted.",
            3 => "Error: Write operation is not permitted.",
            5 => "Error: Insufficient authentication for the operation.",
            6 => "Error: The spec is not supported.",
            7 => "Error: Invalid offset specified in the spec.",
            8 => "Error: Insufficient authorization for the operation.",
            13 => "Error: Invalid attribute length.",
            15 => "Error: Insufficient encryption for the operation.",
            128 => "Error: No resources available.",
            129 => "Error: Internal error occurred.",
            133 => "Error: GATT error.",
            135 => "Error: Attribute not found.",
            137 => "Error: Connection timeout.",
            143 => "Error: Insufficient resources.",
            257 => "Error: Connection failed to establish.",
            _ => "Unknown GATT status."
        };
    }
}
