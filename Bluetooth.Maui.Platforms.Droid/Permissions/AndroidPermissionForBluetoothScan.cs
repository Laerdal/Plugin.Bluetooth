using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Handles the BLUETOOTH_SCAN permission required for scanning Bluetooth devices on Android 12 (API 31) and higher.
/// </summary>
/// <remarks>
///     This permission is required for applications to discover nearby Bluetooth devices.
///     It replaces the deprecated BLUETOOTH_ADMIN permission in Android 12 and higher.
/// </remarks>
[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothScan() : BaseAndroidPermissionHandler(Manifest.Permission.BluetoothScan, true);
