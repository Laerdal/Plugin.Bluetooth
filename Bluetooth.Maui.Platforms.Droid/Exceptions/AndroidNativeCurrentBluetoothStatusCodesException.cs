namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

/// <summary>
///     Represents an exception that occurs when Android Bluetooth operations return a non-success status code.
/// </summary>
/// <remarks>
///     This exception wraps Android's CurrentBluetoothStatusCodes enum values to provide detailed
///     information about why Bluetooth operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeCurrentBluetoothStatusCodesException : AndroidNativeBluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidNativeCurrentBluetoothStatusCodesException" /> class with the specified CurrentBluetoothStatusCodes status and inner exception.
    /// </summary>
    /// <param name="status">The CurrentBluetoothStatusCodes status that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeCurrentBluetoothStatusCodesException(CurrentBluetoothStatusCodes status, Exception? innerException = null) : base($"{CurrentBluetoothStatusCodesToDescription(status)} : {status}", innerException)
    {
        CurrentBluetoothStatusCodes = status;
    }

    /// <summary>
    ///     Gets the specific CurrentBluetoothStatusCodes that caused this exception.
    /// </summary>
    public CurrentBluetoothStatusCodes CurrentBluetoothStatusCodes { get; }

    /// <summary>
    ///     Throws an <see cref="AndroidNativeCurrentBluetoothStatusCodesException" /> if the status is not Success.
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

    private static string CurrentBluetoothStatusCodesToDescription(CurrentBluetoothStatusCodes status)
    {
        var statusCodeValue = (int) status;
        return statusCodeValue switch
        {
            0 => "Success: The operation completed successfully.",
            1 => "Error: Bluetooth is not enabled on this device.",
            2 => "Error: Bluetooth is not allowed for this application.",
            3 => "Error: The device is not bonded.",
            6 => "Error: Missing BLUETOOTH_CONNECT permission.",
            9 => "Error: Profile service is not bound.",
            10 => "Feature is supported.",
            11 => "Feature is not supported.",
            30 => "Feature is not configured.",
            200 => "Error: GATT write is not allowed.",
            201 => "Error: GATT write request is busy.",
            2147483647 => "Error: Unknown error occurred.",
            _ => "Unknown Bluetooth status code."
        };
    }
}
