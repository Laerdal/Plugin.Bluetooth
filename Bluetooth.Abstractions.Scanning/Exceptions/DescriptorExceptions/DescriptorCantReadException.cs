namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Exception thrown when attempting to read from a descriptor that doesn't support read operations.
/// </summary>
public class DescriptorCantReadException : DescriptorException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorCantReadException" /> class.
    /// </summary>
    /// <param name="remoteDescriptor">The descriptor that cannot be read.</param>
    public DescriptorCantReadException(IBluetoothRemoteDescriptor remoteDescriptor)
        : base(remoteDescriptor, $"Descriptor {remoteDescriptor} does not support read operations.")
    {
    }
}