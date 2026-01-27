namespace Bluetooth.Maui.PlatformSpecific.Exceptions;

/// <summary>
/// Represents a Windows-specific native Bluetooth exception that provides detailed error descriptions
/// for various Windows Bluetooth API error conditions.
/// </summary>
public class WindowsNativeBluetoothException : BluetoothException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothException"/> class.
    /// </summary>
    public WindowsNativeBluetoothException() : base("Native Bluetooth Exception")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public WindowsNativeBluetoothException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public WindowsNativeBluetoothException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothException"/> class with a GATT communication status.
    /// </summary>
    /// <param name="status">The GATT communication status that caused the exception.</param>
    public WindowsNativeBluetoothException(GattCommunicationStatus status) : base($"{GattCommunicationStatusToDescription(status)} : {status}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothException"/> class with a GATT communication status and protocol error code.
    /// </summary>
    /// <param name="status">The GATT communication status that caused the exception.</param>
    /// <param name="protocolErrorCode">The protocol error code, if applicable.</param>
    public WindowsNativeBluetoothException(GattCommunicationStatus status, byte? protocolErrorCode) : base($"{GattCommunicationStatusToDescription(status)} : {status}; protocol error code: {protocolErrorCode}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothException"/> class with a Bluetooth error.
    /// </summary>
    /// <param name="error">The Bluetooth error that caused the exception.</param>
    public WindowsNativeBluetoothException(BluetoothError error) : base($"{BluetoothErrorToDescription(error)} : {error}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothException"/> class with a device access status.
    /// </summary>
    /// <param name="deviceAccessStatus">The device access status that caused the exception.</param>
    public WindowsNativeBluetoothException(DeviceAccessStatus deviceAccessStatus) : base($"{DeviceAccessStatusToDescription(deviceAccessStatus)} : {deviceAccessStatus}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsNativeBluetoothException"/> class with a device pairing result status.
    /// </summary>
    /// <param name="status">The device pairing result status that caused the exception.</param>
    public WindowsNativeBluetoothException(DevicePairingResultStatus status) : base($"{DevicePairingResultStatusToDescription(status)} : {status}")
    {
    }

    /// <summary>
    /// Converts a GATT communication status to a human-readable description.
    /// </summary>
    /// <param name="status">The GATT communication status to describe.</param>
    /// <returns>A descriptive string explaining the status.</returns>
    private static string GattCommunicationStatusToDescription(GattCommunicationStatus status)
    {
        return status switch
        {
            GattCommunicationStatus.Unreachable => "No communication can be performed with the device, at this time.",
            GattCommunicationStatus.ProtocolError => "There was a GATT communication protocol error.",
            GattCommunicationStatus.AccessDenied => "Access is denied.",
            _ => $"Unknown GATT communication status: {status}"
        };
    }

    /// <summary>
    /// Converts a device access status to a human-readable description.
    /// </summary>
    /// <param name="deviceAccessStatus">The device access status to describe.</param>
    /// <returns>A descriptive string explaining the status.</returns>
    private static string DeviceAccessStatusToDescription(DeviceAccessStatus deviceAccessStatus)
    {
        return deviceAccessStatus switch
        {
            DeviceAccessStatus.Unspecified => "The device access is not specified.",
            DeviceAccessStatus.Allowed => "Access to the device is allowed.",
            DeviceAccessStatus.DeniedByUser => "Access to the device has been disallowed by the user.",
            DeviceAccessStatus.DeniedBySystem => "Access to the device has been disallowed by the system.",
            _ => $"Unknown device access status: {deviceAccessStatus}"
        };
    }

    /// <summary>
    /// Converts a Bluetooth error to a human-readable description.
    /// </summary>
    /// <param name="error">The Bluetooth error to describe.</param>
    /// <returns>A descriptive string explaining the error.</returns>
    private static string BluetoothErrorToDescription(BluetoothError error)
    {
        return error switch
        {
            BluetoothError.Success => "The operation was successfully completed or serviced.",
            BluetoothError.RadioNotAvailable => "The Bluetooth radio was not available. This error occurs when the Bluetooth radio has been turned off.",
            BluetoothError.ResourceInUse => "The operation cannot be serviced because the necessary resources are currently in use.",
            BluetoothError.DeviceNotConnected => "The operation cannot be completed because the remote device is not connected.",
            BluetoothError.OtherError => "An unexpected error has occurred.",
            BluetoothError.DisabledByPolicy => "The operation is disabled by policy.",
            BluetoothError.NotSupported => "The operation is not supported on the current Bluetooth radio hardware.",
            BluetoothError.DisabledByUser => "The operation is disabled by the user.",
            BluetoothError.ConsentRequired => "The operation requires consent.",
            BluetoothError.TransportNotSupported => "The transport is not supported.",
            _ => $"Unknown error: {error}"
        };
    }

    /// <summary>
    /// Converts a device pairing result status to a human-readable description.
    /// </summary>
    /// <param name="status">The device pairing result status to describe.</param>
    /// <returns>A descriptive string explaining the status.</returns>
    private static string DevicePairingResultStatusToDescription(DevicePairingResultStatus status)
    {
        return status switch
        {
            DevicePairingResultStatus.Paired => "The device object is now paired.",
            DevicePairingResultStatus.NotReadyToPair => "The device object is not in a state where it can be paired.",
            DevicePairingResultStatus.NotPaired => "The device object is not currently paired.",
            DevicePairingResultStatus.AlreadyPaired => "The device object has already been paired.",
            DevicePairingResultStatus.ConnectionRejected => "The device object rejected the connection.",
            DevicePairingResultStatus.TooManyConnections => "The device object indicated it cannot accept any more incoming connections.",
            DevicePairingResultStatus.HardwareFailure => "The device object indicated there was a hardware failure.",
            DevicePairingResultStatus.AuthenticationTimeout => "The authentication process timed out before it could complete.",
            DevicePairingResultStatus.AuthenticationNotAllowed => "The authentication protocol is not supported, so the device is not paired.",
            DevicePairingResultStatus.AuthenticationFailure => "Authentication failed, so the device is not paired. Either the device object or the application rejected the authentication.",
            DevicePairingResultStatus.NoSupportedProfiles => "There are no network profiles for this device object to use.",
            DevicePairingResultStatus.ProtectionLevelCouldNotBeMet => "The minimum level of protection is not supported by the device object or the application.",
            DevicePairingResultStatus.AccessDenied => "Your application does not have the appropriate permissions level to pair the device object.",
            DevicePairingResultStatus.InvalidCeremonyData => "The ceremony data was incorrect.",
            DevicePairingResultStatus.PairingCanceled => "The pairing action was canceled before completion.",
            DevicePairingResultStatus.OperationAlreadyInProgress => "The device object is already attempting to pair or unpair.",
            DevicePairingResultStatus.RequiredHandlerNotRegistered => "Either the event handler wasn't registered or a required DevicePairingKinds was not supported.",
            DevicePairingResultStatus.RejectedByHandler => "The application handler rejected the pairing.",
            DevicePairingResultStatus.RemoteDeviceHasAssociation => "The remove device already has an association.",
            DevicePairingResultStatus.Failed => "An unknown failure occurred.",
            _ => $"Unknown DevicePairingResultStatus: {status}"
        };
    }
}
