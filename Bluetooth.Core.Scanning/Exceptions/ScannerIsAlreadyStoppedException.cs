using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner is already stopped.
/// </summary>
/// <seealso cref="ScannerFailedToStopException" />
public class ScannerIsAlreadyStoppedException : ScannerFailedToStopException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStoppedException"/> class.
    /// </summary>
    public ScannerIsAlreadyStoppedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStoppedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ScannerIsAlreadyStoppedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStoppedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ScannerIsAlreadyStoppedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStoppedException"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerIsAlreadyStoppedException(
        IBluetoothScanner scanner,
        string message = "Scanner is already stopped",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }

    /// <summary>
    ///     Throws an <see cref="ScannerIsAlreadyStoppedException"/> if the Bluetooth scanner is already stopped.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="scanner"/> is null.</exception>
    /// <exception cref="ScannerIsAlreadyStoppedException">Thrown when the scanner is already stopped.</exception>
    public static void ThrowIfIsStopped(IBluetoothScanner scanner)
    {
        ArgumentNullException.ThrowIfNull(scanner);
        if (!scanner.IsRunning)
        {
            throw new ScannerIsAlreadyStoppedException(scanner);
        }
    }
}