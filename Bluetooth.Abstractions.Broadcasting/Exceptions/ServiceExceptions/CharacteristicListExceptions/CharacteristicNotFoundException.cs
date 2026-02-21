namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to access or remove a characteristic that does not exist in a Bluetooth broadcast service.
/// </summary>
/// <seealso cref="IBluetoothLocalService" />
/// <seealso cref="ServiceException" />
public class CharacteristicNotFoundException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException" /> class.
    /// </summary>
    /// <param name="localService">The Bluetooth service associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicNotFoundException(IBluetoothLocalService localService, Exception? innerException = null) : base(localService, "No characteristic have been found matching criteria", innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException" /> class.
    /// </summary>
    /// <param name="localService">The Bluetooth service associated with the exception.</param>
    /// <param name="id">The id of the characteristic that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicNotFoundException(IBluetoothLocalService localService, Guid id, Exception? innerException = null) : base(localService, $"No characteristic have been found for id '{id}'", innerException)
    {
    }
}