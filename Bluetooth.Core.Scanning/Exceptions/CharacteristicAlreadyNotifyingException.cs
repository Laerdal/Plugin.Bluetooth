using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a characteristic is already notifying.
/// </summary>
/// <seealso cref="CharacteristicNotifyException" />
public class CharacteristicAlreadyNotifyingException : CharacteristicNotifyException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyNotifyingException"/> class.
    /// </summary>
    public CharacteristicAlreadyNotifyingException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyNotifyingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicAlreadyNotifyingException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyNotifyingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicAlreadyNotifyingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyNotifyingException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicAlreadyNotifyingException(
        IBluetoothCharacteristic characteristic,
        string message = "Characteristic is already notifying",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }
}