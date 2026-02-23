namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth device is already connected.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceIsAlreadyConnectedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyConnectedException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceIsAlreadyConnectedException(
        IBluetoothRemoteDevice device,
        string message = "Device is already connected",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="DeviceIsAlreadyConnectedException" /> if the device is already connected.
    /// </summary>
    /// <param name="device">The Bluetooth device to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device" /> is null.</exception>
    /// <exception cref="DeviceIsAlreadyConnectedException">Thrown when the device is already connected.</exception>
    public static void ThrowIfAlreadyConnected(IBluetoothRemoteDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (device.IsConnected)
        {
            throw new DeviceIsAlreadyConnectedException(device);
        }
    }
}
