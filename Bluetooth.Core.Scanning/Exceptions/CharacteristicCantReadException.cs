using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to read a characteristic that doesn't support reading.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantReadException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class.
    /// </summary>
    public CharacteristicCantReadException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicCantReadException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicCantReadException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantReadException(
        IBluetoothCharacteristic characteristic,
        string message = "This characteristic does not have the READ property",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantReadException"/> if the characteristic cannot be read.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="characteristic"/> is null.</exception>
    /// <exception cref="CharacteristicCantReadException">Thrown when the characteristic cannot be read.</exception>
    public static void ThrowIfCantRead(IBluetoothCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        if (!characteristic.CanRead)
        {
            throw new CharacteristicCantReadException(characteristic);
        }
    }
}