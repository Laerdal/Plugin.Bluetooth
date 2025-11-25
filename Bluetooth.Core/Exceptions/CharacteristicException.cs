namespace Bluetooth.Core.Exceptions;

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

/// <summary>
///     Represents an exception that occurs during Bluetooth characteristic read operations.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicReadException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicReadException"/> class.
    /// </summary>
    public CharacteristicReadException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicReadException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicReadException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicReadException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicReadException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicReadException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicReadException(
        IBluetoothCharacteristic characteristic,
        string message = "Unknown Bluetooth characteristic read exception",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }
}

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

/// <summary>
///     Represents an exception that occurs when attempting to read a characteristic that doesn't support reading.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantReadException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class.
    /// </summary>
    public CharacteristicCantReadException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicCantReadException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicCantReadException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantReadException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantReadException(
        IBluetoothCharacteristic characteristic,
        string message = "This characteristic does not have the READ property",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantReadException"/> if the characteristic cannot be read.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="characteristic"/> is null.</exception>
    /// <exception cref="CharacteristicCantReadException">Thrown when the characteristic cannot be read.</exception>
    public static void ThrowIfCantRead(IBluetoothCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        if (!characteristic.CanRead)
        {
            throw new CharacteristicCantReadException(characteristic);
        }
    }
}

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

/// <summary>
///     Represents an exception that occurs when attempting to write to a characteristic that doesn't support writing.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantWriteException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException"/> class.
    /// </summary>
    public CharacteristicCantWriteException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicCantWriteException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicCantWriteException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantWriteException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantWriteException(
        IBluetoothCharacteristic characteristic,
        string message = "This characteristic does not have the WRITE property",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantWriteException"/> if the characteristic cannot be written to.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="characteristic"/> is null.</exception>
    /// <exception cref="CharacteristicCantWriteException">Thrown when the characteristic cannot be written to.</exception>
    public static void ThrowIfCantWrite(IBluetoothCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        if (!characteristic.CanWrite)
        {
            throw new CharacteristicCantWriteException(characteristic);
        }
    }
}

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

/// <summary>
///     Represents an exception that occurs when attempting to listen to a characteristic that doesn't support notifications or indications.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class CharacteristicCantListenException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException"/> class.
    /// </summary>
    public CharacteristicCantListenException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicCantListenException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicCantListenException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCantListenException"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicCantListenException(
        IBluetoothCharacteristic characteristic,
        string message = "This characteristic does not have the NOTIFY or INDICATE property",
        Exception? innerException = null)
        : base(characteristic, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="CharacteristicCantListenException"/> if the characteristic cannot be listened to.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="characteristic"/> is null.</exception>
    /// <exception cref="CharacteristicCantListenException">Thrown when the characteristic cannot be listened to.</exception>
    public static void ThrowIfCantListen(IBluetoothCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        if (!characteristic.CanListen)
        {
            throw new CharacteristicCantListenException(characteristic);
        }
    }
}

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
