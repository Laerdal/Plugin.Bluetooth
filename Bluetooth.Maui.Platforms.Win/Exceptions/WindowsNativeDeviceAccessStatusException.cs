namespace Bluetooth.Maui.Platforms.Win.Exceptions;

/// <summary>
///     Represents an exception that occurs when Windows device access status is not allowed.
/// </summary>
/// <remarks>
///     This exception wraps Windows' DeviceAccessStatus enum values to provide detailed
///     information about why device access operations failed.
/// </remarks>
/// <seealso cref="WindowsNativeBluetoothException" />
public class WindowsNativeDeviceAccessStatusException : WindowsNativeBluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsNativeDeviceAccessStatusException" /> class with the specified DeviceAccessStatus and inner exception.
    /// </summary>
    /// <param name="status">The DeviceAccessStatus that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public WindowsNativeDeviceAccessStatusException(DeviceAccessStatus status, Exception? innerException = null)
        : base($"{DeviceAccessStatusToDescription(status)} : {status} ({(int) status})", innerException)
    {
        DeviceAccessStatus = status;
    }

    /// <summary>
    ///     Gets the specific DeviceAccessStatus that caused this exception.
    /// </summary>
    public DeviceAccessStatus DeviceAccessStatus { get; }

    /// <summary>
    ///     Throws a <see cref="WindowsNativeDeviceAccessStatusException" /> if the status is not Allowed.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <exception cref="WindowsNativeDeviceAccessStatusException">Thrown when the status is not Allowed.</exception>
    public static void ThrowIfNotAllowed(DeviceAccessStatus status)
    {
        if (status != DeviceAccessStatus.Allowed)
        {
            throw new WindowsNativeDeviceAccessStatusException(status);
        }
    }

    private static string DeviceAccessStatusToDescription(DeviceAccessStatus deviceAccessStatus)
    {
        var statusCodeValue = (int) deviceAccessStatus;
        return statusCodeValue switch
        {
            0 => "The device access is not specified.",
            1 => "Access to the device is allowed.",
            2 => "Access to the device has been disallowed by the user.",
            3 => "Access to the device has been disallowed by the system.",
            _ => "Unknown device access status."
        };
    }
}
