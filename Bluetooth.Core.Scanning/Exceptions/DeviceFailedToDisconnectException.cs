using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth device fails to disconnect.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceFailedToDisconnectException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class.
    /// </summary>
    public DeviceFailedToDisconnectException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceFailedToDisconnectException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceFailedToDisconnectException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceFailedToDisconnectException(
        IBluetoothDevice device,
        string message = "Failed to disconnect",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}