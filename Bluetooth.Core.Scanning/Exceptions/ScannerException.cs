using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Exceptions;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth scanner operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth scanner associated with the error,
///     allowing for easier debugging and tracking of scanner-related issues.
/// </remarks>
/// <seealso cref="IBluetoothScanner" />
/// <seealso cref="ScannerException" />
public abstract class ScannerException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException"/> class.
    /// </summary>
    protected ScannerException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected ScannerException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ScannerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerException"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ScannerException(
        IBluetoothScanner scanner,
        string message = "Unknown scanner exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        Scanner = scanner;
    }

    /// <summary>
    ///     Gets the Bluetooth scanner associated with the exception.
    /// </summary>
    public IBluetoothScanner? Scanner { get; }
}
