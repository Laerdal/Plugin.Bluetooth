using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Handles the ACCESS_FINE_LOCATION permission required for Bluetooth operations on Android.
/// </summary>
/// <remarks>
///     This permission provides precise location access and is required for Bluetooth scanning operations
///     prior to Android 12 (API 31). It offers more accurate location data than ACCESS_COARSE_LOCATION.
/// </remarks>
public class AndroidPermissionForAccessFineLocation() : BaseAndroidPermissionHandler(Manifest.Permission.AccessFineLocation, true);
