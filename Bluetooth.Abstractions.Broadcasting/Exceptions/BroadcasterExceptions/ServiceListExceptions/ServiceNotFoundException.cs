namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to access or remove a service that does not exist in a Bluetooth broadcaster.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class ServiceNotFoundException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceNotFoundException(IBluetoothBroadcaster broadcaster, Exception? innerException = null) : base(broadcaster, "No service was found matching the criteria",
        innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="id">The UUID of the service that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceNotFoundException(IBluetoothBroadcaster broadcaster, Guid id, Exception? innerException = null) : base(broadcaster,
        $"No service was found for id '{id}'",
        innerException)
    {
    }
}