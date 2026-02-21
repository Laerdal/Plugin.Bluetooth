namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs during Bluetooth characteristic write operations.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicWriteException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicWriteException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="value">The value that was being written when the exception occurred.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicWriteException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        ReadOnlyMemory<byte>? value = null,
        string message = "Unknown Bluetooth characteristic write exception",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
        Value = value;
    }


    /// <summary>
    ///     Gets a read-only collection of the value that was being written when the exception occurred.
    /// </summary>
    public ReadOnlyMemory<byte>? Value { get; }
}