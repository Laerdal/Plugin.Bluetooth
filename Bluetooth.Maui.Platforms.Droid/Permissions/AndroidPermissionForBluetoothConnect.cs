using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothConnect() : BaseAndroidPermissionHandler(Manifest.Permission.BluetoothConnect, true);