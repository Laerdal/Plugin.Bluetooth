using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner fails to stop.
/// </summary>
/// <seealso cref="ScannerException" />
public class ScannerFailedToStopException : ScannerException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerFailedToStopException"/> class.
    /// </summary>
    public ScannerFailedToStopException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerFailedToStopException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ScannerFailedToStopException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerFailedToStopException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ScannerFailedToStopException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerFailedToStopException"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerFailedToStopException(
        IBluetoothScanner scanner,
        string message = "Failed to stop scanner",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }
}