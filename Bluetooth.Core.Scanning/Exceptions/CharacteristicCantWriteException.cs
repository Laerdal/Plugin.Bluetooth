using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to write to a characteristic that doesn't support writing.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantWriteException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException"/> class.
    /// </summary>
    public CharacteristicCantWriteException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicCantWriteException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicCantWriteException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantWriteException(
        IBluetoothCharacteristic characteristic,
        string message = "This characteristic does not have the WRITE property",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantWriteException"/> if the characteristic cannot be written to.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="characteristic"/> is null.</exception>
    /// <exception cref="CharacteristicCantWriteException">Thrown when the characteristic cannot be written to.</exception>
    public static void ThrowIfCantWrite(IBluetoothCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        if (!characteristic.CanWrite)
        {
            throw new CharacteristicCantWriteException(characteristic);
        }
    }
}