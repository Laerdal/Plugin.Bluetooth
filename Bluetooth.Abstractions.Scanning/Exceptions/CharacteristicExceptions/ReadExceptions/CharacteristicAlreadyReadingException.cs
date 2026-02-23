namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a characteristic is already being read.
/// </summary>
/// <seealso cref="CharacteristicReadException" />
public class CharacteristicAlreadyReadingException : CharacteristicReadException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyReadingException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicAlreadyReadingException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "Characteristic is already reading",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
    }
}
