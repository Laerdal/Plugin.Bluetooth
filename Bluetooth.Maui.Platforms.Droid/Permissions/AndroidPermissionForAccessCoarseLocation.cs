using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Handles the ACCESS_COARSE_LOCATION permission required for Bluetooth operations on Android.
/// </summary>
/// <remarks>
///     This permission is required for Bluetooth scanning operations prior to Android 12 (API 31).
///     It provides approximate location access, which is sufficient for discovering nearby Bluetooth devices.
/// </remarks>
public class AndroidPermissionForAccessCoarseLocation() : BaseAndroidPermissionHandler(Manifest.Permission.AccessCoarseLocation, true);
