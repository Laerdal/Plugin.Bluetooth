using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a characteristic is already being read.
/// </summary>
/// <seealso cref="CharacteristicReadException" />
public class CharacteristicAlreadyReadingException : CharacteristicReadException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyReadingException"/> class.
    /// </summary>
    public CharacteristicAlreadyReadingException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyReadingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicAlreadyReadingException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyReadingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicAlreadyReadingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyReadingException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicAlreadyReadingException(
        IBluetoothCharacteristic characteristic,
        string message = "Characteristic is already reading",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }
}