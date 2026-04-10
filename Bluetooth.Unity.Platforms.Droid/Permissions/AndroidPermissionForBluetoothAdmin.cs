using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Handles the BLUETOOTH_ADMIN permission required for Bluetooth administration operations on Android.
/// </summary>
/// <remarks>
///     This permission allows applications to discover and pair Bluetooth devices.
///     It is deprecated in Android 12 (API 31) and above, where BLUETOOTH_SCAN and BLUETOOTH_CONNECT should be used instead.
/// </remarks>
public class AndroidPermissionForBluetoothAdmin() : BaseAndroidPermissionHandler(Manifest.Permission.BluetoothAdmin, false);
