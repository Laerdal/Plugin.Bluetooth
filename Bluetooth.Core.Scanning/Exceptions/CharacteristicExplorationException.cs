using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

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