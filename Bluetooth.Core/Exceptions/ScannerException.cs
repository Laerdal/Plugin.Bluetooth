namespace Bluetooth.Core.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth scanner operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth scanner associated with the error,
///     allowing for easier debugging and tracking of scanner-related issues.
/// </remarks>
/// <seealso cref="IBluetoothScanner" />
/// <seealso cref="ActivityException" />
public abstract class ScannerException : ActivityException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException"/> class.
    /// </summary>
    protected ScannerException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected ScannerException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ScannerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ScannerException(
        IBluetoothScanner scanner,
        string message = "Unknown scanner exception",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when a device is not found during scanning.
/// </summary>
/// <seealso cref="ScannerException" />
public class DeviceNotFoundException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotFoundException"/> class.
    /// </summary>
    public DeviceNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotFoundException"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="id">The device ID that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceNotFoundException(
        IBluetoothScanner scanner,
        string id,
        Exception? innerException = null)
        : base(scanner, FormatDeviceMessage(id), innerException)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        Id = id;
    }

    /// <summary>
    ///     Gets the device ID that was not found.
    /// </summary>
    public string? Id { get; }

    private static string FormatDeviceMessage(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return $"Failed to find the device with id '{id}'";
    }
}

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
