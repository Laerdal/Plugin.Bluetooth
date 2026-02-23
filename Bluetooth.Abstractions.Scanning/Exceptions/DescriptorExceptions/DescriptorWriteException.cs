namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Exception thrown when an error occurs while writing to a Bluetooth descriptor.
/// </summary>
public class DescriptorWriteException : DescriptorException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorWriteException" /> class.
    /// </summary>
    /// <param name="remoteDescriptor">The Bluetooth descriptor associated with the exception.</param>
    /// <param name="value">The value that was being written when the exception occurred.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DescriptorWriteException(
        IBluetoothRemoteDescriptor remoteDescriptor,
        ReadOnlyMemory<byte>? value = null,
        string message = "Unknown Bluetooth descriptor write exception",
        Exception? innerException = null)
        : base(remoteDescriptor, message, innerException)
    {
        Value = value;
    }


    /// <summary>
    ///     Gets a read-only collection of the value that was being written when the exception occurred.
    /// </summary>
    public ReadOnlyMemory<byte>? Value { get; }
}
