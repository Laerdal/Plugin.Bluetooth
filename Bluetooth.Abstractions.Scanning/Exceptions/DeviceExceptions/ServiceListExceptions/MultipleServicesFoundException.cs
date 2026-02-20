namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple services are found matching criteria.
/// </summary>
/// <seealso cref="ServiceExplorationException" />
/// <seealso cref="DeviceException" />
public class MultipleServicesFoundException : ServiceExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="services">The services that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public MultipleServicesFoundException(
        IBluetoothRemoteDevice device,
        IEnumerable<IBluetoothRemoteService> services,
        Exception? innerException = null)
        : base(device, "Multiple services have been found matching criteria", innerException)
    {
        ArgumentNullException.ThrowIfNull(services);
        Services = services;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="id">The id of the services that were found matching the criteria.</param>
    /// <param name="services">The services that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public MultipleServicesFoundException(
        IBluetoothRemoteDevice device,
        Guid id,
        IEnumerable<IBluetoothRemoteService> services,
        Exception? innerException = null)
        : base(device, $"Multiple services have been found with id '{id}'", innerException)
    {
        ArgumentNullException.ThrowIfNull(services);
        Services = services;
    }

    /// <summary>
    ///     Gets the services that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothRemoteService> Services { get; }
}