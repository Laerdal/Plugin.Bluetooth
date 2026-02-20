namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when pairing fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class PairingFailedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PairingFailedException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public PairingFailedException(
        IBluetoothRemoteDevice device,
        string message = "Failed to pair with device",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}