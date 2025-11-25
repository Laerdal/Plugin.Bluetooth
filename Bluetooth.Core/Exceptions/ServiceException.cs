namespace Bluetooth.Core.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth service operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth service associated with the error,
///     allowing for easier debugging and tracking of service-related issues.
/// </remarks>
/// <seealso cref="IBluetoothService" />
public abstract class ServiceException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class.
    /// </summary>
    protected ServiceException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected ServiceException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ServiceException(
        IBluetoothService service,
        string message = "Unknown Bluetooth service exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(service);
        Service = service;
    }

    /// <summary>
    ///     Gets the Bluetooth service associated with the exception.
    /// </summary>
    public IBluetoothService? Service { get; }
}

/// <summary>
///     Represents an exception that occurs when a characteristic is not found.
/// </summary>
/// <seealso cref="ServiceException" />
public class CharacteristicNotFoundException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException"/> class.
    /// </summary>
    public CharacteristicNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with the exception.</param>
    /// <param name="characteristicAddress">The characteristic address that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicNotFoundException(
        IBluetoothService service,
        Guid? characteristicAddress,
        Exception? innerException = null)
        : base(service, FormatCharacteristicMessage(characteristicAddress), innerException)
    {
        CharacteristicAddress = characteristicAddress;
    }

    /// <summary>
    ///     Gets the characteristic address that was not found.
    /// </summary>
    public Guid? CharacteristicAddress { get; }

    private static string FormatCharacteristicMessage(Guid? characteristicAddress)
    {
        return $"Failed to find the Characteristic '{characteristicAddress?.ToString() ?? "NULL"}'";
    }
}

/// <summary>
///     Represents an exception that occurs when characteristic exploration fails.
/// </summary>
/// <seealso cref="ServiceException" />
public class CharacteristicExplorationException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicExplorationException"/> class.
    /// </summary>
    public CharacteristicExplorationException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicExplorationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicExplorationException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicExplorationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicExplorationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicExplorationException"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicExplorationException(
        IBluetoothService service,
        Exception? innerException = null)
        : base(service, "Failed to explore characteristics", innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when multiple characteristics are found matching criteria.
/// </summary>
/// <seealso cref="ServiceException" />
public class MultipleCharacteristicsFoundException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleCharacteristicsFoundException"/> class.
    /// </summary>
    public MultipleCharacteristicsFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleCharacteristicsFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MultipleCharacteristicsFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleCharacteristicsFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MultipleCharacteristicsFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleCharacteristicsFoundException"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with the exception.</param>
    /// <param name="characteristics">The characteristics that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public MultipleCharacteristicsFoundException(
        IBluetoothService service,
        IEnumerable<IBluetoothCharacteristic> characteristics,
        Exception innerException)
        : base(service, "Multiple characteristics have been found matching criteria", innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristics);
        Characteristics = characteristics;
    }

    /// <summary>
    ///     Gets the characteristics that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothCharacteristic>? Characteristics { get; }
}

/// <summary>
///     Represents an exception that occurs when an unexpected characteristic exploration happens.
/// </summary>
/// <seealso cref="ServiceException" />
public class UnexpectedCharacteristicExplorationException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedCharacteristicExplorationException"/> class.
    /// </summary>
    public UnexpectedCharacteristicExplorationException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedCharacteristicExplorationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnexpectedCharacteristicExplorationException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedCharacteristicExplorationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnexpectedCharacteristicExplorationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedCharacteristicExplorationException"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public UnexpectedCharacteristicExplorationException(
        IBluetoothService service,
        string message = "Unexpected characteristic exploration",
        Exception? innerException = null)
        : base(service, message, innerException)
    {
    }
}
