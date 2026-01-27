using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

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