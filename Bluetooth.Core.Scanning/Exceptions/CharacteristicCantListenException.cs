using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to listen to a characteristic that doesn't support notifications or indications.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantListenException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException"/> class.
    /// </summary>
    public CharacteristicCantListenException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicCantListenException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicCantListenException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantListenException(
        IBluetoothCharacteristic characteristic,
        string message = "This characteristic does not have the NOTIFY or INDICATE property",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantListenException"/> if the characteristic cannot be listened to.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="characteristic"/> is null.</exception>
    /// <exception cref="CharacteristicCantListenException">Thrown when the characteristic cannot be listened to.</exception>
    public static void ThrowIfCantListen(IBluetoothCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        if (!characteristic.CanListen)
        {
            throw new CharacteristicCantListenException(characteristic);
        }
    }
}