namespace Bluetooth.Abstractions;

/// <summary>
///     Interface for managing Bluetooth permissions across different platforms.
/// </summary>
/// <remarks>
///     <b>Platform-Specific Behavior:</b>
///     <list type="bullet">
///         <item>
///             <b>Android</b>:
///             <list type="bullet">
///                 <item>API 31+ (Android 12+): Requires runtime permissions BLUETOOTH_SCAN, BLUETOOTH_CONNECT, and BLUETOOTH_ADVERTISE</item>
///                 <item>API 29-30 (Android 10-11): Requires ACCESS_FINE_LOCATION for scanning</item>
///                 <item>Older versions: Requires ACCESS_COARSE_LOCATION for scanning</item>
///                 <item>Permissions can be requested multiple times if denied</item>
///             </list>
///         </item>
///         <item>
///             <b>iOS/macOS</b>:
///             <list type="bullet">
///                 <item>Requires NSBluetoothAlwaysUsageDescription in Info.plist</item>
///                 <item>Broadcasting requires NSBluetoothPeripheralUsageDescription on iOS 12 and older</item>
///                 <item>Permission prompt shown automatically on first Bluetooth access</item>
///                 <item>Once denied, user must manually enable in Settings (cannot re-prompt from app)</item>
///                 <item>iOS 13+: Uses single "Bluetooth Always" permission for all operations</item>
///             </list>
///         </item>
///         <item>
///             <b>Windows</b>:
///             <list type="bullet">
///                 <item>Uses capability-based model (no runtime prompts)</item>
///                 <item>Bluetooth capability must be declared in Package.appxmanifest</item>
///                 <item>All methods return true if capability is declared</item>
///                 <item>Permissions granted automatically at install time</item>
///             </list>
///         </item>
///     </list>
/// </remarks>
public interface IBluetoothPermissionManager
{
    /// <summary>
    ///     Checks if the application has the necessary Bluetooth permissions.
    /// </summary>
    /// <returns>True if permissions are granted, otherwise false.</returns>
    ValueTask<bool> HasBluetoothPermissionsAsync();

    /// <summary>
    ///     Checks if the application has the necessary Bluetooth scanner permissions.
    /// </summary>
    /// <returns>True if scanner permissions are granted, otherwise false.</returns>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: API 31+ checks BLUETOOTH_SCAN + BLUETOOTH_CONNECT; API 29-30 checks FINE_LOCATION; older checks COARSE_LOCATION</item>
    ///         <item><b>iOS/macOS</b>: Same as HasBluetoothPermissionsAsync (no separate permission)</item>
    ///         <item><b>Windows</b>: Always returns true (capability-based)</item>
    ///     </list>
    /// </remarks>
    ValueTask<bool> HasScannerPermissionsAsync();

    /// <summary>
    ///     Checks if the application has the necessary Bluetooth broadcaster permissions.
    /// </summary>
    /// <returns>True if broadcaster permissions are granted, otherwise false.</returns>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: API 31+ checks BLUETOOTH_ADVERTISE + BLUETOOTH_CONNECT; API 29-30 checks FINE_LOCATION; older checks COARSE_LOCATION</item>
    ///         <item><b>iOS/macOS</b>: iOS 13+ checks Bluetooth Always + Peripheral permissions; older versions auto-grant</item>
    ///         <item><b>Windows</b>: Always returns true (capability-based)</item>
    ///     </list>
    /// </remarks>
    ValueTask<bool> HasBroadcasterPermissionsAsync();

    /// <summary>
    ///     Requests the necessary Bluetooth permissions from the user.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>True if permissions are granted, otherwise false.</returns>
    ValueTask<bool> RequestBluetoothPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Requests the necessary Bluetooth scanner permissions from the user.
    /// </summary>
    /// <returns>True if scanner permissions are granted, otherwise false.</returns>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Shows system permission dialog; can be requested multiple times</item>
    ///         <item><b>iOS/macOS</b>: Same as RequestBluetoothPermissionsAsync; once denied, user must enable in Settings</item>
    ///         <item><b>Windows</b>: No-op, always returns true (capability declared at install time)</item>
    ///     </list>
    /// </remarks>
    ValueTask<bool> RequestScannerPermissionsAsync();

    /// <summary>
    ///     Requests the necessary Bluetooth broadcaster permissions from the user.
    /// </summary>
    /// <returns>True if broadcaster permissions are granted, otherwise false.</returns>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Shows system permission dialog; can be requested multiple times</item>
    ///         <item><b>iOS/macOS</b>: Requests both Bluetooth Always and Peripheral permissions; once denied, user must enable in Settings</item>
    ///         <item><b>Windows</b>: No-op, always returns true (capability declared at install time)</item>
    ///     </list>
    /// </remarks>
    ValueTask<bool> RequestBroadcasterPermissionsAsync();
}