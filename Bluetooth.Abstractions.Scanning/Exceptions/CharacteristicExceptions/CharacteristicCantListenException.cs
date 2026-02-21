namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to listen to a characteristic that doesn't support notifications or indications.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantListenException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantListenException(IBluetoothRemoteCharacteristic remoteCharacteristic, string message = "This characteristic does not have the NOTIFY or INDICATE property", Exception? innerException = null) :
        base(remoteCharacteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantListenException" /> if the specified characteristic cannot be listened to.
    /// </summary>
    /// <param name="baseBluetoothRemoteCharacteristic">The Bluetooth characteristic to check.</param>
    public static void ThrowIfCantListen(IBluetoothRemoteCharacteristic baseBluetoothRemoteCharacteristic)
    {
        ArgumentNullException.ThrowIfNull(baseBluetoothRemoteCharacteristic);
        if (!baseBluetoothRemoteCharacteristic.CanListen)
        {
            throw new CharacteristicCantListenException(baseBluetoothRemoteCharacteristic);
        }
    }
}