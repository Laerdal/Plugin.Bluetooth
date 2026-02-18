namespace Bluetooth.Abstractions.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth adapter operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth adapter associated with the error,
///     allowing for easier debugging and tracking of adapter-related issues.
/// </remarks>
public abstract class AdapterException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AdapterException"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected AdapterException(
        IBluetoothAdapter adapter,
        string message = "Unknown adapter exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        Adapter = adapter;
    }

    /// <summary>
    ///     Gets the Bluetooth adapter associated with the exception.
    /// </summary>
    public IBluetoothAdapter? Adapter { get; }
}
