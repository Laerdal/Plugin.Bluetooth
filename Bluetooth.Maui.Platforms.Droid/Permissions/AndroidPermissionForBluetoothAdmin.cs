using Android;

namespace Bluetooth.Maui.Platforms.Droid.Permissions;

public class AndroidPermissionForBluetoothAdmin() : BaseAndroidPermissionHandler(Manifest.Permission.BluetoothAdmin, false);
