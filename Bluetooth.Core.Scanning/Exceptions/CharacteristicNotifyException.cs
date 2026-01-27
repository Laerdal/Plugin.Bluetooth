using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs during Bluetooth characteristic notification operations.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicNotifyException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotifyException"/> class.
    /// </summary>
    public CharacteristicNotifyException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotifyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicNotifyException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotifyException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicNotifyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotifyException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicNotifyException(
        IBluetoothCharacteristic characteristic,
        string message = "Unknown Bluetooth characteristic notify exception",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }
}