namespace Bluetooth.Maui.PlatformSpecific.Permissions;

[SupportedOSPlatform("android31.0")]
public class AndroidPermissionForBluetoothScan() : BaseAndroidPermissionHandler(Android.Manifest.Permission.BluetoothScan, true);
