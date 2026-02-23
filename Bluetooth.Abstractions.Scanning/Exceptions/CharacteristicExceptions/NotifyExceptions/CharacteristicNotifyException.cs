namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs during Bluetooth characteristic notification operations.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicNotifyException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotifyException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicNotifyException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "Unknown Bluetooth characteristic notify exception",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
    }
}
