using Bluetooth.Abstractions;

namespace Bluetooth.Core.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth scanner operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth scanner associated with the error,
///     allowing for easier debugging and tracking of scanner-related issues.
/// </remarks>
public abstract class AdapterException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AdapterException"/> class.
    /// </summary>
    protected AdapterException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AdapterException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected AdapterException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AdapterException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected AdapterException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AdapterException"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected AdapterException(
        IBluetoothAdapter adapter,
        string message = "Unknown scanner exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        Adapter = adapter;
    }

    /// <summary>
    ///     Gets the Bluetooth activity associated with the exception.
    /// </summary>
    public IBluetoothAdapter? Adapter { get; }
}
