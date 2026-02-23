namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a device disconnects unexpectedly.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceUnexpectedDisconnectionException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceUnexpectedDisconnectionException(IBluetoothRemoteDevice device, string message = "Device has disconnected unexpectedly", Exception? innerException = null) : base(device, message, innerException)
    {
    }
}
