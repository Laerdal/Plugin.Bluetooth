using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when parsing a Bluetooth advertisement fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceAdvertisementParsingException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException"/> class.
    /// </summary>
    public DeviceAdvertisementParsingException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceAdvertisementParsingException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceAdvertisementParsingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="advertisement">The advertisement that failed to parse.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceAdvertisementParsingException(
        IBluetoothDevice device,
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