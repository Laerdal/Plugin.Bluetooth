using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

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