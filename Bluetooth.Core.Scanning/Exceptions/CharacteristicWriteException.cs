using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs during Bluetooth characteristic write operations.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicWriteException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicWriteException"/> class.
    /// </summary>
    public CharacteristicWriteException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicWriteException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicWriteException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicWriteException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicWriteException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicWriteException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="value">The value that was being written when the exception occurred.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicWriteException(
        IBluetoothCharacteristic characteristic,
        ReadOnlyMemory<byte>? value = null,
        string message = "Unknown Bluetooth characteristic write exception",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
        Value = value;
    }


    /// <summary>
    ///     Gets a read-only collection of the value that was being written when the exception occurred.
    /// </summary>
    public ReadOnlyMemory<byte>? Value { get; }
}