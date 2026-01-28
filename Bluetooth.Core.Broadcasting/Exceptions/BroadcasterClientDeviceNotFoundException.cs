using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to access or remove a ClientDevice that does not exist in a Bluetooth broadcaster.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class BroadcasterClientDeviceNotFoundException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterClientDeviceNotFoundException"/> class.
    /// </summary>
    public BroadcasterClientDeviceNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterClientDeviceNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BroadcasterClientDeviceNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterClientDeviceNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BroadcasterClientDeviceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterClientDeviceNotFoundException"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="id">The UUID of the ClientDevice that was not found.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BroadcasterClientDeviceNotFoundException(IBluetoothBroadcaster broadcaster, string id, string? message = null, Exception? innerException = null)
        : base(broadcaster, message ?? $"ClientDevice with ID '{id}' does not exist in the broadcaster", innerException)
    {
        ClientDeviceId = id;
    }

    /// <summary>
    ///     Gets the UUID of the ClientDevice that was not found.
    /// </summary>
    public string? ClientDeviceId { get; }
}
