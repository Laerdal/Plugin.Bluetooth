using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Handles the BLUETOOTH_ADVERTISE permission required for Bluetooth advertising operations on Android 12 (API 31) and higher.
/// </summary>
/// <remarks>
///     This permission is required for applications to advertise themselves to nearby Bluetooth devices.
///     It is part of the new runtime permissions model introduced in Android 12.
/// </remarks>
[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothAdvertise() : BaseAndroidPermissionHandler(Manifest.Permission.BluetoothAdvertise, true);
