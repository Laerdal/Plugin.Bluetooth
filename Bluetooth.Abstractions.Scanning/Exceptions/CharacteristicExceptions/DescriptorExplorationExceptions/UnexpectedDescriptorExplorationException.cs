namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when an unexpected Descriptor exploration happens.
/// </summary>
/// <seealso cref="DescriptorExplorationException" />
/// <seealso cref="CharacteristicException" />
public class UnexpectedDescriptorExplorationException : DescriptorExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedDescriptorExplorationException" /> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public UnexpectedDescriptorExplorationException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string? message = null,
        Exception? innerException = null)
        : base(remoteCharacteristic, message ?? "Unexpected descriptor exploration", innerException)
    {
    }
}