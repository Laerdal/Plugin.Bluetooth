namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to add a characteristic that already exists to a Bluetooth broadcast service.
/// </summary>
/// <seealso cref="IBluetoothLocalService" />
/// <seealso cref="ServiceException" />
public class CharacteristicAlreadyExistsException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAlreadyExistsException" /> class.
    /// </summary>
    /// <param name="localService">The Bluetooth broadcast service associated with the exception.</param>
    /// <param name="characteristicId">The UUID of the characteristic that already exists.</param>
    /// <param name="existingLocalCharacteristic">The existing characteristic that caused the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicAlreadyExistsException(IBluetoothLocalService localService,
        Guid characteristicId,
        IBluetoothLocalCharacteristic existingLocalCharacteristic,
        string? message = null,
        Exception? innerException = null) : base(localService, message ?? $"Characteristic with ID '{characteristicId}' already exists in service '{localService?.Name}' ({localService?.Id})", innerException)
    {
        CharacteristicId = characteristicId;
        ExistingLocalCharacteristic = existingLocalCharacteristic;
    }

    /// <summary>
    ///     Gets the UUID of the characteristic that already exists.
    /// </summary>
    public Guid CharacteristicId { get; }

    /// <summary>
    ///     Gets the existing characteristic that caused the exception.
    /// </summary>
    public IBluetoothLocalCharacteristic ExistingLocalCharacteristic { get; }
}