namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected read operation happens.
/// </summary>
/// <seealso cref="CharacteristicReadException" />
public class CharacteristicUnexpectedReadException : CharacteristicReadException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicUnexpectedReadException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "Unexpected characteristic read received",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
    }
}