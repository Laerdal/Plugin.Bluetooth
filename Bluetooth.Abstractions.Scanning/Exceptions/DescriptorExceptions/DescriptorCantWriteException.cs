namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Exception thrown when attempting to write to a descriptor that doesn't support write operations.
/// </summary>
public class DescriptorCantWriteException : DescriptorException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorCantWriteException" /> class.
    /// </summary>
    /// <param name="remoteDescriptor">The descriptor that cannot be written.</param>
    public DescriptorCantWriteException(IBluetoothRemoteDescriptor remoteDescriptor)
        : base(remoteDescriptor, $"Descriptor {remoteDescriptor} does not support write operations.")
    {
    }
}
