namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a characteristic is already being written to.
/// </summary>
/// <seealso cref="CharacteristicWriteException" />
public class CharacteristicAlreadyWritingException : CharacteristicWriteException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyWritingException"/> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="value">The value that was being written when the exception occurred.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicAlreadyWritingException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        ReadOnlyMemory<byte>? value = null,
        string message = "Characteristic is already writing",
        Exception? innerException = null)
        : base(remoteCharacteristic, value, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicAlreadyWritingException"/> if the characteristic is currently being written to.
    /// </summary>
    /// <param name="remoteCharacteristic">The characteristic to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="remoteCharacteristic"/> is null.</exception>
    /// <exception cref="CharacteristicAlreadyWritingException">Thrown when the characteristic is already being written to.</exception>
    public static void ThrowIfAlreadyWriting(IBluetoothRemoteCharacteristic remoteCharacteristic)
    {
        ArgumentNullException.ThrowIfNull(remoteCharacteristic);

        if (remoteCharacteristic.IsWriting)
        {
            throw new CharacteristicAlreadyWritingException(remoteCharacteristic);
        }
    }
}

