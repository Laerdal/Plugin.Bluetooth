namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected characteristic exploration happens.
/// </summary>
/// <seealso cref="CharacteristicExplorationException" />
/// <seealso cref="ServiceException" />
public class UnexpectedCharacteristicExplorationException : CharacteristicExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedCharacteristicExplorationException" /> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public UnexpectedCharacteristicExplorationException(
        IBluetoothRemoteService remoteService,
        string? message = null,
        Exception? innerException = null)
        : base(remoteService, message ?? "Unexpected characteristic exploration", innerException)
    {
    }
}
