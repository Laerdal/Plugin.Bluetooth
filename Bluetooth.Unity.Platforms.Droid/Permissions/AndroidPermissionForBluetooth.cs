using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Handles the BLUETOOTH permission required for basic Bluetooth operations on Android.
/// </summary>
/// <remarks>
///     This permission allows applications to connect to paired Bluetooth devices.
///     It is deprecated in Android 12 (API 31) and above, where BLUETOOTH_CONNECT should be used instead.
/// </remarks>
public class AndroidPermissionForBluetooth() : BaseAndroidPermissionHandler(Manifest.Permission.Bluetooth, false);
