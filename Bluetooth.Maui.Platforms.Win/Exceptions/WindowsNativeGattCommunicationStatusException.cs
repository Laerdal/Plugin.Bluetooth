namespace Bluetooth.Maui.Platforms.Win.Exceptions;

/// <summary>
///     Represents an exception that occurs when Windows GATT communication operations return a non-success status.
/// </summary>
/// <remarks>
///     This exception wraps Windows' GattCommunicationStatus enum values to provide detailed
///     information about why GATT communication operations failed.
/// </remarks>
/// <seealso cref="WindowsNativeBluetoothException" />
public class WindowsNativeGattCommunicationStatusException : WindowsNativeBluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsNativeGattCommunicationStatusException" /> class with the specified GattCommunicationStatus and inner exception.
    /// </summary>
    /// <param name="status">The GattCommunicationStatus that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public WindowsNativeGattCommunicationStatusException(GattCommunicationStatus status, Exception? innerException = null)
        : base($"{GattCommunicationStatusToDescription(status)} : {status} ({(int) status})", innerException)
    {
        GattCommunicationStatus = status;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsNativeGattCommunicationStatusException" /> class with the specified GattCommunicationStatus, protocol error code, and inner exception.
    /// </summary>
    /// <param name="status">The GattCommunicationStatus that caused this exception.</param>
    /// <param name="protocolErrorCode">The protocol error code, if applicable.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public WindowsNativeGattCommunicationStatusException(GattCommunicationStatus status, byte? protocolErrorCode, Exception? innerException = null)
        : base($"{GattCommunicationStatusToDescription(status)} : {status} ({(int) status}); protocol error code: {protocolErrorCode}", innerException)
    {
        GattCommunicationStatus = status;
        ProtocolErrorCode = protocolErrorCode;
    }

    /// <summary>
    ///     Gets the specific GattCommunicationStatus that caused this exception.
    /// </summary>
    public GattCommunicationStatus GattCommunicationStatus { get; }

    /// <summary>
    ///     Gets the protocol error code, if applicable.
    /// </summary>
    public byte? ProtocolErrorCode { get; }

    /// <summary>
    ///     Throws a <see cref="WindowsNativeGattCommunicationStatusException" /> if the status is not Success.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <exception cref="WindowsNativeGattCommunicationStatusException">Thrown when the status is not Success.</exception>
    public static void ThrowIfNotSuccess(GattCommunicationStatus status)
    {
        if (status != GattCommunicationStatus.Success)
        {
            throw new WindowsNativeGattCommunicationStatusException(status);
        }
    }

    /// <summary>
    ///     Throws a <see cref="WindowsNativeGattCommunicationStatusException" /> if the status is not Success.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <param name="protocolErrorCode">The protocol error code, if applicable.</param>
    /// <exception cref="WindowsNativeGattCommunicationStatusException">Thrown when the status is not Success.</exception>
    public static void ThrowIfNotSuccess(GattCommunicationStatus status, byte? protocolErrorCode)
    {
        if (status != GattCommunicationStatus.Success)
        {
            throw new WindowsNativeGattCommunicationStatusException(status, protocolErrorCode);
        }
    }

    private static string GattCommunicationStatusToDescription(GattCommunicationStatus status)
    {
        var statusCodeValue = (int) status;
        return statusCodeValue switch
        {
            0 => "Success: The GATT operation completed successfully.",
            1 => "Error: No communication can be performed with the device at this time.",
            2 => "Error: There was a GATT communication protocol error.",
            3 => "Error: Access is denied.",
            _ => "Unknown GATT communication status."
        };
    }
}
