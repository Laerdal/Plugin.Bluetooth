namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple Descriptors are found matching criteria.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class MultipleDescriptorsFoundException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDescriptorsFoundException" /> class.
    /// </summary>
    /// <param name="localCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="descriptors">The Descriptors that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public MultipleDescriptorsFoundException(IBluetoothLocalCharacteristic localCharacteristic, IEnumerable<IBluetoothLocalDescriptor> descriptors, Exception? innerException = null) : base(localCharacteristic,
        "Multiple Descriptors have been found matching criteria",
        innerException)
    {
        ArgumentNullException.ThrowIfNull(descriptors);
        Descriptors = descriptors;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDescriptorsFoundException" /> class.
    /// </summary>
    /// <param name="localCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="id">The id of the Descriptors that were found matching the criteria.</param>
    /// <param name="descriptors">The Descriptors that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public MultipleDescriptorsFoundException(IBluetoothLocalCharacteristic localCharacteristic, Guid id, IEnumerable<IBluetoothLocalDescriptor> descriptors, Exception? innerException = null) : base(localCharacteristic,
        $"Multiple Descriptors have been found with id '{id}'",
        innerException)
    {
        ArgumentNullException.ThrowIfNull(descriptors);
        Descriptors = descriptors;
    }

    /// <summary>
    ///     Gets the Descriptors that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothLocalDescriptor> Descriptors { get; }
}