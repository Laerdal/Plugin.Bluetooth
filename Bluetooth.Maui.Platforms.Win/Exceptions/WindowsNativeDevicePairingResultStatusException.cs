namespace Bluetooth.Maui.Platforms.Win.Exceptions;

/// <summary>
///     Represents an exception that occurs when Windows device pairing operations return a non-success status.
/// </summary>
/// <remarks>
///     This exception wraps Windows' DevicePairingResultStatus enum values to provide detailed
///     information about why device pairing operations failed.
/// </remarks>
/// <seealso cref="WindowsNativeBluetoothException" />
public class WindowsNativeDevicePairingResultStatusException : WindowsNativeBluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsNativeDevicePairingResultStatusException" /> class with the specified DevicePairingResultStatus and inner exception.
    /// </summary>
    /// <param name="status">The DevicePairingResultStatus that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public WindowsNativeDevicePairingResultStatusException(DevicePairingResultStatus status, Exception? innerException = null)
        : base($"{DevicePairingResultStatusToDescription(status)} : {status} ({(int) status})", innerException)
    {
        DevicePairingResultStatus = status;
    }

    /// <summary>
    ///     Gets the specific DevicePairingResultStatus that caused this exception.
    /// </summary>
    public DevicePairingResultStatus DevicePairingResultStatus { get; }

    /// <summary>
    ///     Throws a <see cref="WindowsNativeDevicePairingResultStatusException" /> if the status is not Paired.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <exception cref="WindowsNativeDevicePairingResultStatusException">Thrown when the status is not Paired.</exception>
    public static void ThrowIfNotPaired(DevicePairingResultStatus status)
    {
        if (status != DevicePairingResultStatus.Paired)
        {
            throw new WindowsNativeDevicePairingResultStatusException(status);
        }
    }

    private static string DevicePairingResultStatusToDescription(DevicePairingResultStatus status)
    {
        var statusCodeValue = (int) status;
        return statusCodeValue switch
        {
            0 => "Success: The device object is now paired.",
            1 => "Error: The device object is not in a state where it can be paired.",
            2 => "Error: The device object is not currently paired.",
            3 => "Error: The device object has already been paired.",
            4 => "Error: The device object rejected the connection.",
            5 => "Error: The device object indicated it cannot accept any more incoming connections.",
            6 => "Error: The device object indicated there was a hardware failure.",
            7 => "Error: The authentication process timed out before it could complete.",
            8 => "Error: The authentication protocol is not supported, so the device is not paired.",
            9 => "Error: Authentication failed, so the device is not paired. Either the device object or the application rejected the authentication.",
            10 => "Error: There are no network profiles for this device object to use.",
            11 => "Error: The minimum level of protection is not supported by the device object or the application.",
            12 => "Error: Your application does not have the appropriate permissions level to pair the device object.",
            13 => "Error: The ceremony data was incorrect.",
            14 => "Error: The pairing action was canceled before completion.",
            15 => "Error: The device object is already attempting to pair or unpair.",
            16 => "Error: Either the event handler wasn't registered or a required DevicePairingKinds was not supported.",
            17 => "Error: The application handler rejected the pairing.",
            18 => "Error: The remote device already has an association.",
            19 => "Error: An unknown failure occurred.",
            _ => "Unknown device pairing result status."
        };
    }
}