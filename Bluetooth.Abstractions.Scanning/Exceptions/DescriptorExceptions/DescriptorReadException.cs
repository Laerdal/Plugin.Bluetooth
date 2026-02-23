namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs during Bluetooth descriptor read operations.
/// </summary>
/// <seealso cref="DescriptorException" />
public class DescriptorReadException : DescriptorException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorReadException" /> class.
    /// </summary>
    /// <param name="remoteDescriptor">The Bluetooth descriptor associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DescriptorReadException(
        IBluetoothRemoteDescriptor remoteDescriptor,
        string message = "Unknown Bluetooth descriptor read exception",
        Exception? innerException = null)
        : base(remoteDescriptor, message, innerException)
    {
    }
}
