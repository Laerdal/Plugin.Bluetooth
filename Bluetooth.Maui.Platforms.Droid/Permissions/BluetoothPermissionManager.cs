namespace Bluetooth.Maui.Platforms.Droid.Permissions;

public class BluetoothPermissionManager : IBluetoothPermissionManager
{
    public async ValueTask<bool> HasBluetoothPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> HasScannerPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> RequestBluetoothPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> RequestScannerPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> RequestBroadcasterPermissionsAsync()
    {
        throw new NotImplementedException();
    }
}