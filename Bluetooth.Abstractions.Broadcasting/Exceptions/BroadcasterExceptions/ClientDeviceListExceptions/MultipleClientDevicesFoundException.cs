namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple ClientDevices with the same ID are found in a Bluetooth broadcaster.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class MultipleClientDevicesFoundException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleClientDevicesFoundException" /> class.
    /// </summary>
    public MultipleClientDevicesFoundException(IBluetoothBroadcaster broadcaster,
        IEnumerable<IBluetoothConnectedDevice> devices,
        Exception? innerException = null)
        : base(broadcaster, "Multiple devices have been found matching criteria", innerException)
    {
        ArgumentNullException.ThrowIfNull(devices);
        ClientDevices = devices;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleClientDevicesFoundException" /> class.
    /// </summary>
    public MultipleClientDevicesFoundException(IBluetoothBroadcaster broadcaster,
        string id,
        IEnumerable<IBluetoothConnectedDevice> devices,
        Exception? innerException = null)
        : base(broadcaster, $"Multiple devices have been found with id '{id}'", innerException)
    {
        ArgumentNullException.ThrowIfNull(devices);
        ClientDevices = devices;
    }

    /// <summary>
    ///     Gets the devices that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothConnectedDevice> ClientDevices { get; }
}
