namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple characteristics are found matching criteria.
/// </summary>
/// <seealso cref="CharacteristicExplorationException" />
/// <seealso cref="ServiceException" />
public class MultipleCharacteristicsFoundException : CharacteristicExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleCharacteristicsFoundException" /> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service associated with the exception.</param>
    /// <param name="characteristics">The characteristics that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public MultipleCharacteristicsFoundException(IBluetoothRemoteService remoteService, IEnumerable<IBluetoothRemoteCharacteristic> characteristics, Exception? innerException = null) : base(remoteService,
        "Multiple characteristics have been found matching criteria",
        innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristics);
        Characteristics = characteristics;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleCharacteristicsFoundException" /> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service associated with the exception.</param>
    /// <param name="id">The id of the characteristics that were found matching the criteria.</param>
    /// <param name="characteristics">The characteristics that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public MultipleCharacteristicsFoundException(IBluetoothRemoteService remoteService, Guid id, IEnumerable<IBluetoothRemoteCharacteristic> characteristics, Exception innerException) : base(remoteService,
        $"Multiple characteristics have been found with id '{id}'",
        innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristics);
        Characteristics = characteristics;
    }

    /// <summary>
    ///     Gets the characteristics that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothRemoteCharacteristic> Characteristics { get; }
}