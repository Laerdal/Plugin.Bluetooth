namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a characteristic is already notifying.
/// </summary>
/// <seealso cref="CharacteristicNotifyException" />
public class CharacteristicAlreadyNotifyingException : CharacteristicNotifyException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyNotifyingException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicAlreadyNotifyingException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "Characteristic is already notifying",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
    }
}