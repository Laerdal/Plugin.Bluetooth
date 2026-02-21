namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a device is not found.
/// </summary>
/// <seealso cref="DeviceExplorationException" />
/// <seealso cref="ScannerException" />
public class DeviceNotFoundException : DeviceExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotFoundException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceNotFoundException(IBluetoothScanner scanner, Exception? innerException = null) : base(scanner, "No device has been found matching criteria", innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotFoundException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="id">The device ID that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceNotFoundException(IBluetoothScanner scanner, string id, Exception? innerException = null) : base(scanner, $"No device has been found for id '{id}'", innerException)
    {
    }
}