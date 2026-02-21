namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth device is already disconnected.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceIsAlreadyDisconnectedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceIsAlreadyDisconnectedException(IBluetoothRemoteDevice device, string message = "Device is already disconnected", Exception? innerException = null) : base(device, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="DeviceIsAlreadyDisconnectedException" /> if the device is already disconnected.
    /// </summary>
    /// <param name="device">The Bluetooth device to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device" /> is null.</exception>
    /// <exception cref="DeviceIsAlreadyDisconnectedException">Thrown when the device is already disconnected.</exception>
    public static void ThrowIfAlreadyDisconnected(IBluetoothRemoteDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (!device.IsConnected)
        {
            throw new DeviceIsAlreadyDisconnectedException(device);
        }
    }
}