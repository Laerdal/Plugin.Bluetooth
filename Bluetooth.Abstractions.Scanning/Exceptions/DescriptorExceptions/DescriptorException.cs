namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Base exception class for descriptor-related errors.
/// </summary>
public class DescriptorException : BluetoothScanningException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorException" /> class.
    /// </summary>
    /// <param name="remoteDescriptor">The descriptor associated with this exception.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public DescriptorException(IBluetoothRemoteDescriptor remoteDescriptor, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        RemoteDescriptor = remoteDescriptor;
    }

    /// <summary>
    ///     Gets the descriptor associated with this exception.
    /// </summary>
    public IBluetoothRemoteDescriptor RemoteDescriptor { get; }
}
