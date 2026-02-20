namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth device operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth device associated with the error,
///     allowing for easier debugging and tracking of device-related issues.
/// </remarks>
/// <seealso cref="IBluetoothRemoteDevice" />
public abstract class DeviceException : BluetoothScanningException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected DeviceException(
        IBluetoothRemoteDevice device,
        string message = "Unknown Bluetooth device exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(device);
        Device = device;
    }

    /// <summary>
    ///     Gets the Bluetooth device associated with the exception.
    /// </summary>
    public IBluetoothRemoteDevice Device { get; }
}