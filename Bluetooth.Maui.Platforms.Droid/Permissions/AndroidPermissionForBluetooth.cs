using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

public class AndroidPermissionForBluetooth() : BaseAndroidPermissionHandler(Manifest.Permission.Bluetooth, false);