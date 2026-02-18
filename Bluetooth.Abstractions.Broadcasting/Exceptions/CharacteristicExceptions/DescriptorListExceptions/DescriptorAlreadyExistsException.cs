namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to add a descriptor that already exists to a Bluetooth broadcast characteristic.
/// </summary>
/// <seealso cref="IBluetoothLocalCharacteristic" />
/// <seealso cref="CharacteristicException" />
public class DescriptorAlreadyExistsException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="localCharacteristic">The Bluetooth broadcast characteristic associated with the exception.</param>
    /// <param name="descriptorId">The UUID of the descriptor that already exists.</param>
    /// <param name="existingLocalDescriptor">The existing descriptor that caused the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DescriptorAlreadyExistsException(IBluetoothLocalCharacteristic localCharacteristic,
        Guid descriptorId,
        IBluetoothLocalDescriptor existingLocalDescriptor,
        string? message = null,
        Exception? innerException = null) : base(localCharacteristic, message ?? $"Descriptor with ID '{descriptorId}' already exists in characteristic '{localCharacteristic?.Name}' ({localCharacteristic?.Id})", innerException)
    {
        DescriptorId = descriptorId;
        ExistingLocalDescriptor = existingLocalDescriptor;
    }

    /// <summary>
    ///     Gets the UUID of the descriptor that already exists.
    /// </summary>
    public Guid DescriptorId { get; }

    /// <summary>
    ///     Gets the existing descriptor that caused the exception.
    /// </summary>
    public IBluetoothLocalDescriptor ExistingLocalDescriptor { get; }

}
