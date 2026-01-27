using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Exceptions;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth service operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth service associated with the error,
///     allowing for easier debugging and tracking of service-related issues.
/// </remarks>
/// <seealso cref="IBluetoothService" />
public abstract class ServiceException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class.
    /// </summary>
    protected ServiceException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected ServiceException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ServiceException(
        IBluetoothService service,
        string message = "Unknown Bluetooth service exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(service);
        Service = service;
    }

    /// <summary>
    ///     Gets the Bluetooth service associated with the exception.
    /// </summary>
    public IBluetoothService? Service { get; }
}