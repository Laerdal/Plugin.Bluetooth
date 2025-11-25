namespace Bluetooth.Maui.PlatformSpecific.Permissions;

[SupportedOSPlatform("android29.0")]
public class AndroidPermissionForAccessBackgroundLocation() : BaseAndroidPermissionHandler(Android.Manifest.Permission.AccessBackgroundLocation, true);
