namespace Bluetooth.Maui.Platforms.Droid.Exceptions;

/// <summary>
/// Represents an exception that occurs when Android Bluetooth LE advertising fails.
/// </summary>
/// <remarks>
/// This exception wraps Android's AdvertiseFailure enum values to provide detailed
/// information about why Bluetooth LE advertising operations failed.
/// </remarks>
/// <seealso cref="AndroidNativeBluetoothException" />
public class AndroidNativeAdvertiseFailureException : AndroidNativeBluetoothException
{
    /// <summary>
    /// Gets the specific AdvertiseFailure that caused this exception.
    /// </summary>
    public AdvertiseFailure AdvertiseFailure { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNativeAdvertiseFailureException"/> class with the specified AdvertiseFailure status and inner exception.
    /// </summary>
    /// <param name="status">The AdvertiseFailure status that caused this exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public AndroidNativeAdvertiseFailureException(AdvertiseFailure status, Exception? innerException = null)
        : base($"{AdvertiseFailureToDescription(status)} : {status}", innerException)
    {
        AdvertiseFailure = status;
    }

    private static string AdvertiseFailureToDescription(AdvertiseFailure status)
    {
        var statusCodeValue = (int)status;
        return statusCodeValue switch
        {
            1 => "Error: The advertised data is too large.",
            2 => "Error: Too many advertisers are already active.",
            3 => "Error: An internal error occurred during advertising.",
            4 => "Error: Advertising is not supported on this device.",
            5 => "Error: Advertising has already been started.",
            _ => "Unknown advertising failure."
        };
    }
}
