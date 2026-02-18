namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected write notification is received.
/// </summary>
/// <seealso cref="CharacteristicNotifyException" />
public class CharacteristicUnexpectedWriteNotifyException : CharacteristicNotifyException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteNotifyException"/> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicUnexpectedWriteNotifyException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "Unexpected characteristic write notify received",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
    }
}
