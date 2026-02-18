namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a device is in the process of reconnecting.
/// </summary>
public class DeviceReconnectingException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceReconnectingException"/> class.
    /// </summary>
    /// <param name="device"> The Bluetooth device associated with the exception.</param>
    /// <param name="message"> A message that describes the error.</param>
    /// <param name="innerException"> The inner exception that caused the current exception, if any.</param>
    public DeviceReconnectingException(IBluetoothRemoteDevice device, string message = "Device is reconnecting", Exception? innerException = null) : base(device, message, innerException)
    {
    }
}