namespace Bluetooth.Maui.Platforms.Droid.Permissions;

[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothAdvertise() : BaseAndroidPermissionHandler(Android.Manifest.Permission.BluetoothAdvertise, true);
