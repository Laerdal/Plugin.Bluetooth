using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth device is not connected.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceNotConnectedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException"/> class.
    /// </summary>
    public DeviceNotConnectedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceNotConnectedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceNotConnectedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceNotConnectedException(
        IBluetoothDevice device,
        string message = "Device needs to be connected to execute this operation",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="DeviceNotConnectedException"/> if the device is not connected.
    /// </summary>
    /// <param name="device">The Bluetooth device to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    public static void ThrowIfNotConnected(IBluetoothDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (!device.IsConnected)
        {
            throw new DeviceNotConnectedException(device);
        }
    }
}