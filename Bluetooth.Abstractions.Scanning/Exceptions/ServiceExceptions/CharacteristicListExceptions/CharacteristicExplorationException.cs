namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when characteristic exploration fails.
/// </summary>
/// <seealso cref="ServiceException" />
public class CharacteristicExplorationException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicExplorationException" /> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicExplorationException(
        IBluetoothRemoteService remoteService,
        string? message = null,
        Exception? innerException = null)
        : base(remoteService, message ?? "Unknown failure while exploring characteristics", innerException)
    {
    }
}