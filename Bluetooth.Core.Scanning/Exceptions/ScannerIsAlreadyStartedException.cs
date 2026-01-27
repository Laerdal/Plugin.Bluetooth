using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a Bluetooth scanner is already started.
/// </summary>
/// <seealso cref="ScannerFailedToStartException" />
public class ScannerIsAlreadyStartedException : ScannerFailedToStartException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStartedException"/> class.
    /// </summary>
    public ScannerIsAlreadyStartedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStartedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ScannerIsAlreadyStartedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStartedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ScannerIsAlreadyStartedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerIsAlreadyStartedException"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ScannerIsAlreadyStartedException(
        IBluetoothScanner scanner,
        string message = "Scanner is already started",
        Exception? innerException = null)
        : base(scanner, message, innerException)
    {
    }

    /// <summary>
    ///     Throws an <see cref="ScannerIsAlreadyStartedException"/> if the Bluetooth scanner is already started.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="scanner"/> is null.</exception>
    /// <exception cref="ScannerIsAlreadyStartedException">Thrown when the scanner is already running.</exception>
    public static void ThrowIfIsStarted(IBluetoothScanner scanner)
    {
        ArgumentNullException.ThrowIfNull(scanner);
        if (scanner.IsRunning)
        {
            throw new ScannerIsAlreadyStartedException(scanner);
        }
    }
}