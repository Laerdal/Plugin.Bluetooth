namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to write to a characteristic that doesn't support writing.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantWriteException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantWriteException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "This characteristic does not have the WRITE property",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantWriteException" /> if the specified characteristic cannot be written to.
    /// </summary>
    /// <param name="baseBluetoothRemoteCharacteristic">The Bluetooth characteristic to check.</param>
    public static void ThrowIfCantWrite(IBluetoothRemoteCharacteristic baseBluetoothRemoteCharacteristic)
    {
        ArgumentNullException.ThrowIfNull(baseBluetoothRemoteCharacteristic);
        if (!baseBluetoothRemoteCharacteristic.CanWrite)
        {
            throw new CharacteristicCantWriteException(baseBluetoothRemoteCharacteristic);
        }
    }
}