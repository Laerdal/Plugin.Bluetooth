namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected write operation happens.
/// </summary>
/// <seealso cref="CharacteristicWriteException" />
public class CharacteristicUnexpectedWriteException : CharacteristicWriteException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="value">The value that was being written when the exception occurred.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicUnexpectedWriteException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        ReadOnlyMemory<byte>? value = null,
        string message = "Unexpected characteristic write received",
        Exception? innerException = null)
        : base(remoteCharacteristic, value, message, innerException)
    {
    }
}
