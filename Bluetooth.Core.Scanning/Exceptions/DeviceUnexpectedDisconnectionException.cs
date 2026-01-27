using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a device disconnects unexpectedly.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceUnexpectedDisconnectionException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException"/> class.
    /// </summary>
    public DeviceUnexpectedDisconnectionException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceUnexpectedDisconnectionException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceUnexpectedDisconnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceUnexpectedDisconnectionException(
        IBluetoothDevice device,
        string message = "Device has disconnected unexpectedly",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}