namespace Bluetooth.Maui.Platforms.DotNetCore.Permissions;

/// <inheritdoc />
public class BluetoothPermissionManager : IBluetoothPermissionManager
{
    /// <inheritdoc />
    public ValueTask<bool> HasBluetoothPermissionsAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    public ValueTask<bool> HasScannerPermissionsAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    public ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    public ValueTask<bool> RequestBluetoothPermissionsAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    public ValueTask<bool> RequestScannerPermissionsAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    public ValueTask<bool> RequestBroadcasterPermissionsAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}