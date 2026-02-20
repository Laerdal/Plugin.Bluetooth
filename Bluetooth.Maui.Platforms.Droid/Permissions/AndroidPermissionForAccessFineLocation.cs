using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

public class AndroidPermissionForAccessFineLocation() : BaseAndroidPermissionHandler(Manifest.Permission.AccessFineLocation, true);