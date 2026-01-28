namespace Bluetooth.Maui.Platforms.Apple.PlatformSpecific.Exceptions;

/// <summary>
/// Represents an exception that occurs in Apple-specific native Bluetooth operations.
/// </summary>
/// <remarks>
/// This exception wraps Apple's NSError objects to provide detailed information
/// about iOS/macOS Bluetooth operation failures, including error codes, domains,
/// descriptions, and recovery suggestions.
/// </remarks>
/// <seealso cref="BluetoothException" />
public class AppleNativeBluetoothException : BluetoothException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppleNativeBluetoothException"/> class.
    /// </summary>
    public AppleNativeBluetoothException()
    {
        Code = 0;
        Domain = string.Empty;
        Description = string.Empty;
        FailureReason = string.Empty;
        RecoveryOptions = string.Empty;
        RecoverySuggestion = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleNativeBluetoothException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AppleNativeBluetoothException(string message) : base(message)
    {
        Code = 0;
        Domain = string.Empty;
        Description = message;
        FailureReason = string.Empty;
        RecoveryOptions = string.Empty;
        RecoverySuggestion = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleNativeBluetoothException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AppleNativeBluetoothException(string message, Exception innerException) : base(message, innerException)
    {
        Code = 0;
        Domain = string.Empty;
        Description = message;
        FailureReason = string.Empty;
        RecoveryOptions = string.Empty;
        RecoverySuggestion = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleNativeBluetoothException"/> class with details from an NSError.
    /// </summary>
    /// <param name="nsError">The NSError containing the native error details.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="nsError"/> is null.</exception>
    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    public AppleNativeBluetoothException(NSError nsError) : base($"Native Bluetooth Exception: {nsError?.LocalizedDescription ?? "Unknown error"} ({nsError?.Code ?? 0})")
    {
        ArgumentNullException.ThrowIfNull(nsError);

        Code = (int)nsError.Code;
        Domain = nsError.Domain;
        Description = nsError.LocalizedDescription;
        FailureReason = nsError.LocalizedFailureReason;
        RecoveryOptions = string.Join(";", nsError.LocalizedRecoveryOptions);
        RecoverySuggestion = nsError.LocalizedRecoverySuggestion;
    }

    /// <summary>
    /// Throws an <see cref="AppleNativeBluetoothException"/> if the provided NSError is not null.
    /// </summary>
    /// <param name="nsError">The NSError to check. If not null, an exception will be thrown.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when <paramref name="nsError"/> is not null.</exception>
    public static void ThrowIfError(NSError? nsError)
    {
        if (nsError != null)
        {
            throw new AppleNativeBluetoothException(nsError);
        }
    }

    /// <summary>
    /// Gets the error code from the NSError.
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Gets the error domain from the NSError.
    /// </summary>
    public string Domain { get; }

    /// <summary>
    /// Gets the localized description of the error.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the localized failure reason for the error.
    /// </summary>
    public string FailureReason { get; }

    /// <summary>
    /// Gets the semicolon-separated list of localized recovery options for the error.
    /// </summary>
    public string RecoveryOptions { get; }

    /// <summary>
    /// Gets the localized recovery suggestion for the error.
    /// </summary>
    public string RecoverySuggestion { get; }
}
