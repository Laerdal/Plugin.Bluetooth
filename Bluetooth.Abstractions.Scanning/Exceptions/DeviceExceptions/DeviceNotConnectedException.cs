namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth device is not connected.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceNotConnectedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceNotConnectedException(
        IBluetoothRemoteDevice device,
        string message = "Device needs to be connected to execute this operation",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="DeviceNotConnectedException" /> if the specified device is not connected.
    /// </summary>
    /// <param name="serviceDevice">The Bluetooth device to check.</param>
    public static void ThrowIfNotConnected(IBluetoothRemoteDevice serviceDevice)
    {
        ArgumentNullException.ThrowIfNull(serviceDevice);
        if (!serviceDevice.IsConnected)
        {
            throw new DeviceNotConnectedException(serviceDevice);
        }
    }
}
