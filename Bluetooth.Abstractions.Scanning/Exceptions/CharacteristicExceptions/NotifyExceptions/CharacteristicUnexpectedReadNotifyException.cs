namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected read notification is received.
/// </summary>
/// <seealso cref="CharacteristicNotifyException" />
public class CharacteristicUnexpectedReadNotifyException : CharacteristicNotifyException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadNotifyException"/> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicUnexpectedReadNotifyException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "Unexpected characteristic read notify received",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
    }
}
