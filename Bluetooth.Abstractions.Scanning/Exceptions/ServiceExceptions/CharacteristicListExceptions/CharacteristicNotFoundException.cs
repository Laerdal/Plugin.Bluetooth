namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a characteristic is not found.
/// </summary>
/// <seealso cref="CharacteristicExplorationException" />
/// <seealso cref="ServiceException" />
public class CharacteristicNotFoundException : CharacteristicExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException" /> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicNotFoundException(IBluetoothRemoteService remoteService, Exception? innerException = null) : base(remoteService, "No characteristic have been found matching criteria", innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException" /> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service associated with the exception.</param>
    /// <param name="id">The id of the characteristic that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicNotFoundException(IBluetoothRemoteService remoteService, Guid id, Exception? innerException = null) : base(remoteService, $"No characteristic have been found for id '{id}'", innerException)
    {
    }
}