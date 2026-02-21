namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Descriptor is not found.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class DescriptorNotFoundException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorNotFoundException" /> class.
    /// </summary>
    /// <param name="localCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DescriptorNotFoundException(IBluetoothLocalCharacteristic localCharacteristic, Exception? innerException = null) : base(localCharacteristic, "No descriptor has been found matching criteria", innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorNotFoundException" /> class.
    /// </summary>
    /// <param name="localCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="id">The descriptor ID that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DescriptorNotFoundException(IBluetoothLocalCharacteristic localCharacteristic, Guid id, Exception? innerException = null) : base(localCharacteristic, $"No descriptor has been found for id '{id}'", innerException)
    {
    }
}