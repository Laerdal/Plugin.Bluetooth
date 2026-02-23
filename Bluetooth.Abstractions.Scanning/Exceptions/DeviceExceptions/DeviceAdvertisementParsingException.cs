namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when parsing a Bluetooth advertisement fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceAdvertisementParsingException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="advertisement">The advertisement that failed to parse.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceAdvertisementParsingException(
        IBluetoothRemoteDevice device,
        IBluetoothAdvertisement advertisement,
        string message = "Failed to parse advertisement",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
        ArgumentNullException.ThrowIfNull(advertisement);
        Advertisement = advertisement;
    }

    /// <summary>
    ///     Gets the advertisement that failed to parse.
    /// </summary>
    public IBluetoothAdvertisement? Advertisement { get; }
}
