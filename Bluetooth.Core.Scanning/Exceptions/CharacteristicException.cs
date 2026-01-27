using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Exceptions;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth characteristic operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth characteristic associated with the error,
///     allowing for easier debugging and tracking of characteristic-related issues.
/// </remarks>
/// <seealso cref="IBluetoothCharacteristic" />
public abstract class CharacteristicException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicException"/> class.
    /// </summary>
    protected CharacteristicException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected CharacteristicException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected CharacteristicException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected CharacteristicException(
        IBluetoothCharacteristic characteristic,
        string message = "Unknown Bluetooth characteristic exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        Characteristic = characteristic;
    }

    /// <summary>
    ///     Gets the Bluetooth characteristic associated with the exception.
    /// </summary>
    public IBluetoothCharacteristic? Characteristic { get; }
}