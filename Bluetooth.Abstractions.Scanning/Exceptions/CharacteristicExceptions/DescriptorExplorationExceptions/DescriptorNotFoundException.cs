namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Descriptor is not found.
/// </summary>
/// <seealso cref="DescriptorExplorationException" />
/// <seealso cref="CharacteristicException" />
public class DescriptorNotFoundException : DescriptorExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorNotFoundException"/> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DescriptorNotFoundException(IBluetoothRemoteCharacteristic remoteCharacteristic, Exception? innerException = null) : base(remoteCharacteristic, $"No descriptor has been found matching criteria", innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorNotFoundException"/> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="id">The descriptor ID that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DescriptorNotFoundException(IBluetoothRemoteCharacteristic remoteCharacteristic, Guid id, Exception? innerException = null) : base(remoteCharacteristic, $"No descriptor has been found for id '{id}'", innerException)
    {
    }

}

