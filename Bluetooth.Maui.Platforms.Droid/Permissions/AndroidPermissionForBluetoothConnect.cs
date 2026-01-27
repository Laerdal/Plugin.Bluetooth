namespace Bluetooth.Maui.Platforms.Droid.Permissions;

[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothConnect() : BaseAndroidPermissionHandler(Android.Manifest.Permission.BluetoothConnect, true);
