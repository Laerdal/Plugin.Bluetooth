using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when multiple services are found matching criteria.
/// </summary>
/// <seealso cref="DeviceException" />
public class MultipleServicesFoundException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException"/> class.
    /// </summary>
    public MultipleServicesFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MultipleServicesFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MultipleServicesFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="services">The services that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public MultipleServicesFoundException(
        IBluetoothDevice device,
        IEnumerable<IBluetoothService> services,
        Exception? innerException = null)
        : base(device, "Multiple services have been found matching criteria", innerException)
    {
        ArgumentNullException.ThrowIfNull(services);
        Services = services;
    }

    /// <summary>
    ///     Gets the services that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothService>? Services { get; }
}