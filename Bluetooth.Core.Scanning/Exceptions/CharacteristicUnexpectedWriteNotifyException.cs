using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected write notification is received.
/// </summary>
/// <seealso cref="CharacteristicNotifyException" />
public class CharacteristicUnexpectedWriteNotifyException : CharacteristicNotifyException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteNotifyException"/> class.
    /// </summary>
    public CharacteristicUnexpectedWriteNotifyException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteNotifyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicUnexpectedWriteNotifyException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteNotifyException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicUnexpectedWriteNotifyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedWriteNotifyException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicUnexpectedWriteNotifyException(
        IBluetoothCharacteristic characteristic,
        string message = "Unexpected characteristic write notify received",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }
}