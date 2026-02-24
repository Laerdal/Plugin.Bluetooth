namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth characteristic is found in an unexpected service,
///     rather than the intended one.
/// </summary>
/// <remarks>
///     This exception provides detailed information about the characteristic, the unexpected service ID in which it was
///     found,
///     and the expected service ID. This information helps in identifying configuration or connection issues between
///     Bluetooth
///     services and their characteristics.
/// </remarks>
/// <example>
///     <code>
/// try
/// {
///     // Code attempting to locate a Bluetooth characteristic in a specific service
/// }
/// catch (CharacteristicFoundInWrongServiceException ex)
/// {
///     Console.WriteLine(ex.Message);
/// }
/// </code>
/// </example>
/// <seealso cref="IBluetoothCharacteristicAccessService" />
/// <seealso cref="IBluetoothCharacteristic" />
/// <seealso cref="CharacteristicAccessServiceException" />
public class CharacteristicFoundInWrongServiceException : CharacteristicAccessServiceException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicFoundInWrongServiceException"/> class.
    /// </summary>
    /// <param name="characteristicAccessService">The intended characteristic service.</param>
    /// <param name="characteristic">The characteristic found in the wrong service.</param>
    /// <param name="serviceId">The unique identifier of the service in which the characteristic was incorrectly found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicFoundInWrongServiceException(
        IBluetoothCharacteristicAccessService characteristicAccessService,
        IBluetoothCharacteristic characteristic,
        Guid serviceId,
        Exception? innerException = null)
        : base(characteristicAccessService, $"The characteristic {characteristic} was found in an unexpected service : {serviceId}", innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristicAccessService);
        ArgumentNullException.ThrowIfNull(characteristic);

        Characteristic = characteristic;
        ServiceId = serviceId;
    }

    /// <summary>
    ///     Gets the characteristic found in the wrong service.
    /// </summary>
    public IBluetoothCharacteristic? Characteristic { get; }

    /// <summary>
    ///     Gets the unique identifier of the service in which the characteristic was incorrectly found.
    /// </summary>
    public Guid ServiceId { get; }
}
