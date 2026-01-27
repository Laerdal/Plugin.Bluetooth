namespace Bluetooth.Maui.Platforms.Apple.Permissions;

/// <inheritdoc/>
public class BluetoothPermissionManager : IBluetoothPermissionManager
{
    /// <inheritdoc/>
    public ValueTask<bool> HasBluetoothPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ValueTask<bool> HasScannerPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ValueTask<bool> RequestBluetoothPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ValueTask<bool> RequestScannerPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ValueTask<bool> RequestBroadcasterPermissionsAsync()
    {
        throw new NotImplementedException();
    }
}
