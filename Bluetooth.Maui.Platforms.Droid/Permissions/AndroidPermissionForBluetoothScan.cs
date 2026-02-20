using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothScan() : BaseAndroidPermissionHandler(Manifest.Permission.BluetoothScan, true);