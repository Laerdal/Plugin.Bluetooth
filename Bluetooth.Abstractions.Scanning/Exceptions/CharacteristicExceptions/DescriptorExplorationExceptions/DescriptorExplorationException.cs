namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when Descriptor exploration fails.
/// </summary>
/// <seealso cref="CharacteristicException" />
public class DescriptorExplorationException : CharacteristicException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorExplorationException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth Characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DescriptorExplorationException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string? message = null,
        Exception? innerException = null)
        : base(remoteCharacteristic, message ?? "Unknown failure while exploring descriptors", innerException)
    {
    }
}