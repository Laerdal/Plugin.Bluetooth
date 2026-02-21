namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to access or remove a ClientDevice that does not exist in a Bluetooth broadcaster.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class ClientDeviceNotFoundException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ClientDeviceNotFoundException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ClientDeviceNotFoundException(IBluetoothBroadcaster broadcaster, Exception? innerException = null) : base(broadcaster, "No ClientDevice was found matching the criteria",
        innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ClientDeviceNotFoundException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="id">The UUID of the ClientDevice that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ClientDeviceNotFoundException(IBluetoothBroadcaster broadcaster, string id, Exception? innerException = null) : base(broadcaster,
        $"No ClientDevice was found for id '{id}'",
        innerException)
    {
    }
}