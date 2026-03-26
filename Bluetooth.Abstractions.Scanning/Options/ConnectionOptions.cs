using Bluetooth.Abstractions.Options;
using Bluetooth.Abstractions.Scanning.Options.Android;
using Bluetooth.Abstractions.Scanning.Options.Apple;
using Bluetooth.Abstractions.Scanning.Options.Windows;

namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents Bluetooth device connection options.
/// </summary>
public record ConnectionOptions
{
    #region Permission Handling

    /// <summary>
    ///     Gets the permission request strategy for this connection operation.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see cref="PermissionRequestStrategy.RequestAutomatically"/> which automatically
    ///     requests BLUETOOTH_CONNECT permission (Android 12+) before connecting if not already granted.
    /// </remarks>
    public PermissionRequestStrategy PermissionStrategy { get; init; } = PermissionRequestStrategy.RequestAutomatically;

    #endregion

    /// <summary>
    ///     Gets a value indicating whether to wait for an advertisement before connecting to the device.
    /// </summary>
    public bool WaitForAdvertisementBeforeConnecting { get; init; }

    #region Retry Configuration

    /// <summary>
    ///     Gets the retry configuration for device connection operations.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Retry configuration applied when initial connection attempts fail due to transient issues.
    ///         Critical for Android GATT error 133 (connection failures), which is common and often
    ///         resolves with retry.
    ///     </para>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Retries critical for GATT error 133 (connection failures)</item>
    ///         <item><b>iOS/macOS</b>: Retries on connection timeout or peripheral unavailable</item>
    ///         <item><b>Windows</b>: Retries on device connection failures</item>
    ///     </list>
    ///     <para>
    ///         Defaults to <see cref="RetryOptions.Default"/> (3 retries with 200ms delay).
    ///         Set to <see cref="RetryOptions.None"/> to disable retry logic.
    ///     </para>
    /// </remarks>
    public RetryOptions? ConnectionRetry { get; init; } = RetryOptions.Default;

    #endregion

    #region Platform-Specific Options

    /// <summary>
    ///     Gets the Apple platform-specific connection options.
    /// </summary>
    /// <remarks>
    ///     These options are only used on iOS/macOS platforms and are ignored on other platforms.
    /// </remarks>
    public AppleConnectionOptions? Apple { get; init; }

    /// <summary>
    ///     Gets the Android platform-specific connection options.
    /// </summary>
    /// <remarks>
    ///     These options are only used on Android platforms and are ignored on other platforms.
    /// </remarks>
    public AndroidConnectionOptions? Android { get; init; }

    /// <summary>
    ///     Gets the Windows platform-specific connection options.
    /// </summary>
    /// <remarks>
    ///     These options are only used on Windows platforms and are ignored on other platforms.
    /// </remarks>
    public WindowsConnectionOptions? Windows { get; init; }

    #endregion
}
