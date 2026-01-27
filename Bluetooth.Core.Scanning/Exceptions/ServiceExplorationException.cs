using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when service exploration fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class ServiceExplorationException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException"/> class.
    /// </summary>
    public ServiceExplorationException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ServiceExplorationException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ServiceExplorationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceExplorationException(
        IBluetoothDevice device,
        Exception? innerException = null)
        : base(device, "Failed to explore services", innerException)
    {
    }
}