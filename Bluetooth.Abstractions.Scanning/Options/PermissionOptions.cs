namespace Bluetooth.Abstractions.Scanning.Options;

public record PermissionOptions
{
    /// <summary>
    ///     Gets a value indicating whether background location permission should be requested on Android.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: API 29-30 (Android 10-11) requests ACCESS_BACKGROUND_LOCATION if true; required for background scanning</item>
    ///         <item><b>iOS/macOS</b>: Ignored (background permissions handled by Info.plist)</item>
    ///         <item><b>Windows</b>: Ignored (no background permission needed)</item>
    ///     </list>
    ///     Defaults to false (foreground scanning only).
    /// </remarks>
    public bool RequireBackgroundLocation { get; init; }

    /// <summary>
    ///     Gets the permission request strategy for this scanning operation.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see cref="PermissionRequestStrategy.RequestAutomatically"/> which automatically
    ///     requests permissions before starting the scan if not already granted.
    /// </remarks>
    public PermissionRequestStrategy PermissionStrategy { get; init; } = PermissionRequestStrategy.RequestAutomatically;

}
