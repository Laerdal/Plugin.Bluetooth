namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth device fails to disconnect.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceFailedToDisconnectException : DeviceException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceFailedToDisconnectException(
        IBluetoothRemoteDevice device,
        string message = "Failed to disconnect",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}
