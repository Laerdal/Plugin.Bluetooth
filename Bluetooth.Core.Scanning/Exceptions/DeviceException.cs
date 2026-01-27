using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Exceptions;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth device operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth device associated with the error,
///     allowing for easier debugging and tracking of device-related issues.
/// </remarks>
/// <seealso cref="IBluetoothDevice" />
public abstract class DeviceException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException"/> class.
    /// </summary>
    protected DeviceException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected DeviceException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected DeviceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected DeviceException(
        IBluetoothDevice device,
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
    public IBluetoothDevice? Device { get; }
}