using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth device is already disconnected.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceIsAlreadyDisconnectedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException"/> class.
    /// </summary>
    public DeviceIsAlreadyDisconnectedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceIsAlreadyDisconnectedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceIsAlreadyDisconnectedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceIsAlreadyDisconnectedException(
        IBluetoothDevice device,
        string message = "Device is already disconnected",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="DeviceIsAlreadyDisconnectedException"/> if the device is already disconnected.
    /// </summary>
    /// <param name="device">The Bluetooth device to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    /// <exception cref="DeviceIsAlreadyDisconnectedException">Thrown when the device is already disconnected.</exception>
    public static void ThrowIfAlreadyDisconnected(IBluetoothDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (!device.IsConnected)
        {
            throw new DeviceIsAlreadyDisconnectedException(device);
        }
    }
}