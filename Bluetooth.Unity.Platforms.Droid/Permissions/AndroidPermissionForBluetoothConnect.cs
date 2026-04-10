using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Handles the BLUETOOTH_CONNECT permission required for connecting to Bluetooth devices on Android 12 (API 31) and higher.
/// </summary>
/// <remarks>
///     This permission is required for applications to connect to already paired Bluetooth devices.
///     It replaces the deprecated BLUETOOTH permission in Android 12 and higher.
/// </remarks>
[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothConnect() : BaseAndroidPermissionHandler(Manifest.Permission.BluetoothConnect, true);
