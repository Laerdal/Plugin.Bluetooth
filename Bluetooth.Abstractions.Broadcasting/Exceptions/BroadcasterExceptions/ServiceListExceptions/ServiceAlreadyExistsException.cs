namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when attempting to add a service that already exists to a Bluetooth broadcaster.
/// </summary>
public class ServiceAlreadyExistsException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceAlreadyExistsException" /> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="serviceId">The UUID of the service that already exists.</param>
    /// <param name="existingLocalService"> The existing service that caused the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceAlreadyExistsException(IBluetoothBroadcaster broadcaster,
        Guid serviceId,
        IBluetoothLocalService existingLocalService,
        string? message = null,
        Exception? innerException = null) : base(broadcaster, message ?? $"Service with ID '{serviceId}' already exists in the broadcaster", innerException)
    {
        ServiceId = serviceId;
        ExistingLocalService = existingLocalService;
    }

    /// <summary>
    ///     Gets the UUID of the service that already exists.
    /// </summary>
    public Guid ServiceId { get; }

    /// <summary>
    ///     Gets the existing service that caused the exception.
    /// </summary>
    public IBluetoothLocalService ExistingLocalService { get; }
}
