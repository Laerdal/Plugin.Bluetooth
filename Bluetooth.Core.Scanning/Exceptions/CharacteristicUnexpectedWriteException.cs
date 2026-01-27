using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected write operation happens.
/// </summary>
/// <seealso cref="CharacteristicWriteException" />
public class CharacteristicUnexpectedWriteException : CharacteristicWriteException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteException"/> class.
    /// </summary>
    public CharacteristicUnexpectedWriteException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicUnexpectedWriteException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicUnexpectedWriteException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="value">The value that was being written when the exception occurred.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicUnexpectedWriteException(
        IBluetoothCharacteristic characteristic,
        ReadOnlyMemory<byte>? value = null,
        string message = "Unexpected characteristic write received",
        Exception? innerException = null)
        : base(characteristic, value, message, innerException)
    {
    }
}