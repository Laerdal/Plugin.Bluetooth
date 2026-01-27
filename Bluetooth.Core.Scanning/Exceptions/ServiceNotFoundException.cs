using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a service is not found.
/// </summary>
/// <seealso cref="DeviceException" />
public class ServiceNotFoundException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    public ServiceNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ServiceNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ServiceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="serviceAddress">The service address that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceNotFoundException(
        IBluetoothDevice device,
        Guid? serviceAddress,
        Exception? innerException = null)
        : base(device, FormatServiceMessage(serviceAddress), innerException)
    {
        ServiceAddress = serviceAddress;
    }

    /// <summary>
    ///     Gets the service address that was not found.
    /// </summary>
    public Guid? ServiceAddress { get; }

    private static string FormatServiceMessage(Guid? serviceAddress)
    {
        return $"Failed to find the Service '{serviceAddress?.ToString() ?? "NULL"}'";
    }
}