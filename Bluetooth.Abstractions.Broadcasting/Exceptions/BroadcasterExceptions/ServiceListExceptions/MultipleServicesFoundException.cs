namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple services are found matching criteria.
/// </summary>
/// <seealso cref="BroadcasterException" />
public class MultipleServicesFoundException : BroadcasterException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException" /> class.
    /// </summary>
    public MultipleServicesFoundException(IBluetoothBroadcaster broadcaster,
        IEnumerable<IBluetoothLocalService> services,
        Exception? innerException = null)
        : base(broadcaster, "Multiple services have been found matching criteria", innerException)
    {
        ArgumentNullException.ThrowIfNull(services);
        Services = services;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException" /> class.
    /// </summary>
    public MultipleServicesFoundException(IBluetoothBroadcaster broadcaster,
        Guid id,
        IEnumerable<IBluetoothLocalService> services,
        Exception? innerException = null)
        : base(broadcaster, $"Multiple services have been found with id '{id}'", innerException)
    {
        ArgumentNullException.ThrowIfNull(services);
        Services = services;
    }

    /// <summary>
    ///     Gets the services that caused the exception.
    /// </summary>
    public IEnumerable<IBluetoothLocalService> Services { get; }
}
