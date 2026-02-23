namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple devices are found matching criteria.
/// </summary>
/// <seealso cref="DeviceExplorationException" />
/// <seealso cref="ScannerException" />
public class MultipleDevicesFoundException : DeviceExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDevicesFoundException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="devices">The devices that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public MultipleDevicesFoundException(IBluetoothScanner scanner, IEnumerable<IBluetoothRemoteDevice> devices, Exception innerException) : base(scanner, "Multiple devices have been found matching criteria", innerException)
    {
        ArgumentNullException.ThrowIfNull(devices);
        Devices = devices;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDevicesFoundException" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="id">The id of the devices that were found matching the criteria.</param>
    /// <param name="devices">The devices that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public MultipleDevicesFoundException(IBluetoothScanner scanner, string id, IEnumerable<IBluetoothRemoteDevice> devices, Exception innerException) : base(scanner, $"Multiple devices have been found with id '{id}'", innerException)
    {
        ArgumentNullException.ThrowIfNull(devices);
        Devices = devices;
    }

    /// <summary>
    ///     Gets the devices that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothRemoteDevice> Devices { get; }
}
