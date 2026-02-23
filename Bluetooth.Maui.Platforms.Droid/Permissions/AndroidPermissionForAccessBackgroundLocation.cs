using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Handles the ACCESS_BACKGROUND_LOCATION permission required for Bluetooth scanning in the background on Android 10 (API 29) and higher.
/// </summary>
/// <remarks>
///     This permission is required when an app needs to access location while running in the background,
///     which is necessary for Bluetooth scanning operations when the app is not in the foreground.
/// </remarks>
[SupportedOSPlatform("android29.0")]
public class AndroidPermissionForAccessBackgroundLocation() : BaseAndroidPermissionHandler(Manifest.Permission.AccessBackgroundLocation, true);
