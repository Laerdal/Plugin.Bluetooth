using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple devices are found matching criteria.
/// </summary>
/// <seealso cref="ScannerException" />
public class MultipleDevicesFoundException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDevicesFoundException"/> class.
    /// </summary>
    public MultipleDevicesFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDevicesFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MultipleDevicesFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDevicesFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MultipleDevicesFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDevicesFoundException"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="devices">The devices that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public MultipleDevicesFoundException(
        IBluetoothScanner scanner,
        IEnumerable<IBluetoothDevice> devices,
        Exception innerException)
        : base(scanner, "Multiple devices have been found matching criteria", innerException)
    {
        ArgumentNullException.ThrowIfNull(devices);
        Devices = devices;
    }

    /// <summary>
    ///     Gets the devices that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothDevice>? Devices { get; }
}