namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when service exploration fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class ServiceExplorationException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceExplorationException(
        IBluetoothRemoteDevice device,
        string? message = null,
        Exception? innerException = null)
        : base(device, message ?? "Unknown failure while exploring devices", innerException)
    {
    }
}