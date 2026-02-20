namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when bonding fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class BondingFailedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BondingFailedException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BondingFailedException(
        IBluetoothRemoteDevice device,
        string message = "Bonding failed",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}