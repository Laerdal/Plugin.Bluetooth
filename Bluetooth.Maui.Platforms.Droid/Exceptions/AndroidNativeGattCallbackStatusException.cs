using Bluetooth.Maui.Platforms.Droid.Enums;

namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

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
    /// Initializes a new instance of the <see cref="AndroidNativeGattCallbackStatusException"/> class with the specified GattCallbackStatus and inner exception.
    /// </summary>
    /// <param name="status">The GattCallbackStatus that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeGattCallbackStatusException(GattCallbackStatus status, Exception? innerException = null)
        : base($"{GattCallbackStatusToDescription(status)} : {status}", innerException)
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

    private static string GattCallbackStatusToDescription(GattCallbackStatus status)
    {
        var statusCodeValue = (int)status;
        return statusCodeValue switch
        {
            0x00 => "Success: The GATT operation completed successfully.",
            0x01 => "Error: Invalid GATT handle.",
            0x02 => "Error: GATT read operation not permitted.",
            0x03 => "Error: GATT write operation not permitted.",
            0x04 => "Error: Invalid GATT PDU.",
            0x05 => "Error: Insufficient authentication for GATT operation.",
            0x06 => "Error: GATT request not supported.",
            0x07 => "Error: Invalid offset in GATT request.",
            0x08 => "Error: Insufficient authorization for GATT operation.",
            0x09 => "Error: GATT prepare queue full.",
            0x0a => "Error: GATT attribute not found.",
            0x0b => "Error: GATT attribute not long.",
            0x0c => "Error: Insufficient encryption key size for GATT operation.",
            0x0d => "Error: Invalid GATT attribute length.",
            0x0e => "Error: Unlikely GATT error.",
            0x0f => "Error: Insufficient encryption for GATT operation.",
            0x10 => "Error: Unsupported GATT group type.",
            0x11 => "Error: Insufficient GATT resources.",
            0x22 => "Error: GATT connection LMP timeout.",
            0x3A => "Error: GATT controller busy.",
            0x3B => "Error: Unacceptable GATT connection interval.",
            0x80 => "Error: No GATT resources available.",
            0x81 => "Error: Internal GATT error.",
            0x82 => "Error: GATT wrong state.",
            0x83 => "Error: GATT database full.",
            0x84 => "Error: GATT busy.",
            0x85 => "Error: Generic GATT error.",
            0x86 => "Error: GATT command started.",
            0x87 => "Error: Illegal GATT parameter.",
            0x88 => "Error: GATT operation pending.",
            0x89 => "Error: GATT authentication failed.",
            0x8a => "Error: More GATT data available.",
            0x8b => "Error: Invalid GATT configuration.",
            0x8c => "Error: GATT service already started.",
            0x8d => "Error: GATT encrypted but no MITM protection.",
            0x8e => "Error: GATT not encrypted.",
            0x8f => "Error: GATT congested.",
            0xFD => "Error: GATT CCCD configuration error.",
            0xFE => "Error: GATT procedure already in progress.",
            0xFF => "Error: GATT value out of range.",
            _ => "Unknown GATT callback status code."
        };
    }
}
