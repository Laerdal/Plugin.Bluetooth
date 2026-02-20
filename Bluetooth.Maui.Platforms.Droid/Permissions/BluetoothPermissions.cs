namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Provides Android-specific Bluetooth permissions for the Bluetooth.Maui library.
/// </summary>
public static class BluetoothPermissions
{
    #region Permissions

    /// <summary>
    ///     Gets the permission required for Bluetooth scanning on Android 12+ (API level 31+).
    ///     This permission allows the app to discover and scan for nearby Bluetooth devices.
    /// </summary>
    [SupportedOSPlatform("android31.0")]
    public static AndroidPermissionForBluetoothScan BluetoothScanPermission => new();

    /// <summary>
    ///     Gets the permission required for Bluetooth advertising on Android 12+ (API level 31+).
    ///     This permission allows the app to advertise as a Bluetooth device to other devices.
    /// </summary>
    [SupportedOSPlatform("android31.0")]
    public static AndroidPermissionForBluetoothAdvertise BluetoothAdvertisePermission => new();

    /// <summary>
    ///     Gets the permission required for Bluetooth connections on Android 12+ (API level 31+).
    ///     This permission allows the app to connect to paired Bluetooth devices.
    /// </summary>
    [SupportedOSPlatform("android31.0")]
    public static AndroidPermissionForBluetoothConnect BluetoothConnectPermission => new();

    /// <summary>
    ///     Gets the basic Bluetooth permission for Android devices.
    ///     This permission allows the app to use Bluetooth functionality.
    /// </summary>
    public static AndroidPermissionForBluetooth BluetoothPermission => new();

    /// <summary>
    ///     Gets the Bluetooth admin permission for Android devices.
    ///     This permission allows the app to initiate device discovery and make the device discoverable.
    /// </summary>
    public static AndroidPermissionForBluetoothAdmin BluetoothAdminPermission => new();

    /// <summary>
    ///     Gets the fine location permission for Android devices.
    ///     This permission is required for Bluetooth scanning and device discovery on Android 6.0+ (API level 23+).
    /// </summary>
    public static AndroidPermissionForAccessFineLocation FineLocationPermission => new();

    /// <summary>
    ///     Gets the coarse location permission for Android devices.
    ///     This permission is required for Bluetooth functionality on some Android versions.
    /// </summary>
    public static AndroidPermissionForAccessCoarseLocation CoarseLocationPermission => new();

    /// <summary>
    ///     Gets the background location permission for Android 10+ (API level 29+).
    ///     This permission is required when accessing location services while the app runs in the background.
    /// </summary>
    [SupportedOSPlatform("android29.0")]
    public static AndroidPermissionForAccessBackgroundLocation BackgroundLocationPermission => new();

    #endregion
}