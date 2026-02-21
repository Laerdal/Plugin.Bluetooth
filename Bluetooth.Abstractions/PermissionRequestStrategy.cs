namespace Bluetooth.Abstractions;

/// <summary>
///     Defines when and how Bluetooth permissions should be requested.
/// </summary>
public enum PermissionRequestStrategy
{
    /// <summary>
    ///     Automatically request permissions when starting scanner/broadcaster or connecting to device.
    ///     Recommended for most applications - provides best developer experience.
    /// </summary>
    /// <remarks>
    ///     Platform behavior:
    ///     <list type="bullet">
    ///         <item><b>iOS/macOS</b>: No-op (CoreBluetooth shows system dialog automatically)</item>
    ///         <item><b>Android</b>: Requests permissions before operation</item>
    ///         <item><b>Windows</b>: Checks adapter state and requests radio access if needed</item>
    ///     </list>
    /// </remarks>
    RequestAutomatically = 0, // Default

    /// <summary>
    ///     Throw BluetoothPermissionException if permissions not granted.
    ///     Use when you want explicit control over permission dialogs.
    /// </summary>
    /// <remarks>
    ///     Requires developer to call IBluetoothPermissionManager.RequestXXXPermissionsAsync()
    ///     before starting scanner/broadcaster or connecting to device.
    /// </remarks>
    ThrowIfNotGranted = 1,

    /// <summary>
    ///     Assume permissions are granted, skip all checks.
    ///     Use only when you've manually requested permissions elsewhere.
    /// </summary>
    /// <remarks>
    ///     ⚠️ WARNING: If permissions are not actually granted:
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Will throw SecurityException from native code</item>
    ///         <item><b>iOS</b>: System will show permission dialog (cannot be prevented)</item>
    ///         <item><b>Windows</b>: May fail when accessing Bluetooth APIs</item>
    ///     </list>
    /// </remarks>
    AssumeGranted = 2
}