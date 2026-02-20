using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

[SupportedOSPlatform("android29.0")]
public class AndroidPermissionForAccessBackgroundLocation() : BaseAndroidPermissionHandler(Manifest.Permission.AccessBackgroundLocation, true);