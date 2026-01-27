using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected read notification is received.
/// </summary>
/// <seealso cref="CharacteristicNotifyException" />
public class CharacteristicUnexpectedReadNotifyException : CharacteristicNotifyException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadNotifyException"/> class.
    /// </summary>
    public CharacteristicUnexpectedReadNotifyException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadNotifyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicUnexpectedReadNotifyException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadNotifyException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicUnexpectedReadNotifyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicUnexpectedReadNotifyException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicUnexpectedReadNotifyException(
        IBluetoothCharacteristic characteristic,
        string message = "Unexpected characteristic read notify received",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }
}