namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple Descriptors are found matching criteria.
/// </summary>
/// <seealso cref="DescriptorExplorationException" />
/// <seealso cref="CharacteristicException" />
public class MultipleDescriptorsFoundException : DescriptorExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDescriptorsFoundException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="descriptors">The Descriptors that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public MultipleDescriptorsFoundException(IBluetoothRemoteCharacteristic remoteCharacteristic, IEnumerable<IBluetoothRemoteDescriptor> descriptors, Exception? innerException = null) : base(remoteCharacteristic,
        "Multiple Descriptors have been found matching criteria",
        innerException)
    {
        ArgumentNullException.ThrowIfNull(descriptors);
        Descriptors = descriptors;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleDescriptorsFoundException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="id">The id of the Descriptors that were found matching the criteria.</param>
    /// <param name="descriptors">The Descriptors that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public MultipleDescriptorsFoundException(IBluetoothRemoteCharacteristic remoteCharacteristic, Guid id, IEnumerable<IBluetoothRemoteDescriptor> descriptors, Exception? innerException = null) : base(remoteCharacteristic,
        $"Multiple Descriptors have been found with id '{id}'",
        innerException)
    {
        ArgumentNullException.ThrowIfNull(descriptors);
        Descriptors = descriptors;
    }

    /// <summary>
    ///     Gets the Descriptors that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothRemoteDescriptor> Descriptors { get; }
}