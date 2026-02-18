namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to read a characteristic that doesn't support reading.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantReadException : CharacteristicException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantReadException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "This characteristic does not have the READ property",
        Exception? innerException = null)
        : base(remoteCharacteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantReadException"/> if the specified characteristic cannot be read.
    /// </summary>
    /// <param name="baseBluetoothRemoteCharacteristic">The Bluetooth characteristic to check.</param>
    public static void ThrowIfCantRead(IBluetoothRemoteCharacteristic baseBluetoothRemoteCharacteristic)
    {
        ArgumentNullException.ThrowIfNull(baseBluetoothRemoteCharacteristic);
        if (!baseBluetoothRemoteCharacteristic.CanRead)
        {
            throw new CharacteristicCantReadException(baseBluetoothRemoteCharacteristic);
        }
    }
}
