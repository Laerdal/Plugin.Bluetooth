using Exception = System.Exception;

namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

/// <summary>
/// Represents an exception that occurs when Android Bluetooth LE scanning fails.
/// </summary>
/// <remarks>
/// This exception wraps Android's ScanFailure enum values to provide detailed
/// information about why Bluetooth LE scanning operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeScanFailureException : AndroidNativeBluetoothException
{
    /// <summary>
    /// Gets the specific ScanFailure that caused this exception.
    /// </summary>
    public ScanFailure ScanFailure { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeScanFailureException"/> class with the specified ScanFailure status and inner exception.
    /// </summary>
    /// <param name="status">The ScanFailure status that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeScanFailureException(ScanFailure status, Exception? innerException = null)
        : base($"{ScanFailureToDescription(status)} : {status}", innerException)
    {
        ScanFailure = status;
    }

    private static string ScanFailureToDescription(ScanFailure status)
    {
        var statusCodeValue = (int)status;
        return statusCodeValue switch
        {
            1 => "Error: Scanning has already been started.",
            2 => "Error: Failed to register the application for scanning.",
            3 => "Error: An internal error occurred during scanning.",
            4 => "Error: Scanning is not supported on this device.",
            5 => "Error: Scanning is being started too frequently.",
            6 => "Error: Scanning is out of hardware resources.",
            _ => "Unknown scan failure."
        };
    }
}
