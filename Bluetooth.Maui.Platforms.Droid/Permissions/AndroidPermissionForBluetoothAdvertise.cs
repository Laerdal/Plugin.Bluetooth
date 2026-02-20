using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothAdvertise() : BaseAndroidPermissionHandler(Manifest.Permission.BluetoothAdvertise, true);