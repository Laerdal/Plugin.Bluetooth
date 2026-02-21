namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a service is not found.
/// </summary>
/// <seealso cref="ServiceExplorationException" />
/// <seealso cref="DeviceException" />
public class ServiceNotFoundException : ServiceExplorationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceNotFoundException(IBluetoothRemoteDevice device, Exception? innerException = null) : base(device, "No service have been found matching criteria", innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="id">The id of the service that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceNotFoundException(IBluetoothRemoteDevice device, Guid id, Exception? innerException = null) : base(device, $"No service have been found for id '{id}'", innerException)
    {
    }
}