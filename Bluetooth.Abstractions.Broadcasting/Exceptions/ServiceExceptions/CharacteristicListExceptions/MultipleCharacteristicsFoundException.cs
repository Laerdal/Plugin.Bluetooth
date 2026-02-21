namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple characteristics are found matching criteria.
/// </summary>
/// <seealso cref="IBluetoothLocalService" />
/// <seealso cref="ServiceException" />
public class MultipleCharacteristicsFoundException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleCharacteristicsFoundException" /> class.
    /// </summary>
    /// <param name="localService">The Bluetooth service associated with the exception.</param>
    /// <param name="characteristics">The characteristics that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public MultipleCharacteristicsFoundException(IBluetoothLocalService localService, IEnumerable<IBluetoothLocalCharacteristic> characteristics, Exception? innerException = null) : base(localService,
        "Multiple characteristics have been found matching criteria",
        innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristics);
        Characteristics = characteristics;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleCharacteristicsFoundException" /> class.
    /// </summary>
    /// <param name="localService">The Bluetooth service associated with the exception.</param>
    /// <param name="id">The id of the characteristics that were found matching the criteria.</param>
    /// <param name="characteristics">The characteristics that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public MultipleCharacteristicsFoundException(IBluetoothLocalService localService, Guid id, IEnumerable<IBluetoothLocalCharacteristic> characteristics, Exception innerException) : base(localService,
        $"Multiple characteristics have been found with id '{id}'",
        innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristics);
        Characteristics = characteristics;
    }

    /// <summary>
    ///     Gets the characteristics that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothLocalCharacteristic> Characteristics { get; }
}