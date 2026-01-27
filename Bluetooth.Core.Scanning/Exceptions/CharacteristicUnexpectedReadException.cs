using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected read operation happens.
/// </summary>
/// <seealso cref="CharacteristicReadException" />
public class CharacteristicUnexpectedReadException : CharacteristicReadException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadException"/> class.
    /// </summary>
    public CharacteristicUnexpectedReadException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicUnexpectedReadException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicUnexpectedReadException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicUnexpectedReadException(
        IBluetoothCharacteristic characteristic,
        string message = "Unexpected characteristic read received",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }
}