using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a characteristic is already being written to.
/// </summary>
/// <seealso cref="CharacteristicWriteException" />
public class CharacteristicAlreadyWritingException : CharacteristicWriteException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyWritingException"/> class.
    /// </summary>
    public CharacteristicAlreadyWritingException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyWritingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicAlreadyWritingException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyWritingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicAlreadyWritingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyWritingException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="value">The value that was being written when the exception occurred.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicAlreadyWritingException(
        IBluetoothCharacteristic characteristic,
        ReadOnlyMemory<byte>? value = null,
        string message = "Characteristic is already writing",
        Exception? innerException = null)
        : base(characteristic, value, message, innerException)
    {
    }
}