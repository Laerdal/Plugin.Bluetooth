using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

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