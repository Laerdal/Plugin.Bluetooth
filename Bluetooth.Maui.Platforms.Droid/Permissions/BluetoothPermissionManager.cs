namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <inheritdoc />
public class BluetoothPermissionManager : IBluetoothPermissionManager
{
    /// <inheritdoc />
    public async ValueTask<bool> HasBluetoothPermissionsAsync()
    {
        try
        {
            // For API 31+ (Android 12+), need BLUETOOTH_CONNECT
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                var status = await BluetoothPermissions.BluetoothConnectPermission.CheckStatusAsync().ConfigureAwait(false);
                return status == PermissionStatus.Granted;
            }

            // For older versions, need location permissions
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                var status = await BluetoothPermissions.FineLocationPermission.CheckStatusAsync().ConfigureAwait(false);
                return status == PermissionStatus.Granted;
            }

            var coarseStatus = await BluetoothPermissions.CoarseLocationPermission.CheckStatusAsync().ConfigureAwait(false);
            return coarseStatus == PermissionStatus.Granted;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> HasScannerPermissionsAsync()
    {
        try
        {
            await BluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

            // For API 31+ (Android 12+), need BLUETOOTH_SCAN and BLUETOOTH_CONNECT
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                var scanStatus = await BluetoothPermissions.BluetoothScanPermission.CheckStatusAsync().ConfigureAwait(false);
                var connectStatus = await BluetoothPermissions.BluetoothConnectPermission.CheckStatusAsync().ConfigureAwait(false);
                return scanStatus == PermissionStatus.Granted &&
                       connectStatus == PermissionStatus.Granted;
            }

            // For API 29-30 (Android 10-11), need FINE_LOCATION
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                var status = await BluetoothPermissions.FineLocationPermission.CheckStatusAsync().ConfigureAwait(false);
                return status == PermissionStatus.Granted;
            }

            // For older versions, COARSE_LOCATION is sufficient
            var coarseStatus = await BluetoothPermissions.CoarseLocationPermission.CheckStatusAsync().ConfigureAwait(false);
            return coarseStatus == PermissionStatus.Granted;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        try
        {
            await BluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

            // For API 31+ (Android 12+), need BLUETOOTH_ADVERTISE
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                var status = await BluetoothPermissions.BluetoothAdvertisePermission.CheckStatusAsync().ConfigureAwait(false);
                return status == PermissionStatus.Granted;
            }

            // For older versions, advertising doesn't need special permissions beyond basic Bluetooth
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> RequestBluetoothPermissionsAsync()
    {
        try
        {
            await BluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

            // For API 31+ (Android 12+), request BLUETOOTH_CONNECT
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                await BluetoothPermissions.BluetoothConnectPermission.RequestIfNeededAsync().ConfigureAwait(false);
                return true;
            }

            // For older versions, request location permissions
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                await BluetoothPermissions.FineLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
                return true;
            }

            await BluetoothPermissions.CoarseLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> RequestScannerPermissionsAsync()
    {
        try
        {
            await BluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

            // For API 31+ (Android 12+), request BLUETOOTH_SCAN and BLUETOOTH_CONNECT
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                await BluetoothPermissions.BluetoothScanPermission.RequestIfNeededAsync().ConfigureAwait(false);
                await BluetoothPermissions.BluetoothConnectPermission.RequestIfNeededAsync().ConfigureAwait(false);
                return true;
            }

            // For API 29-30 (Android 10-11), request FINE_LOCATION and optionally BACKGROUND_LOCATION
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                await BluetoothPermissions.FineLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
                // For using Bluetooth LE in Background (optional, may be denied by user)
                try
                {
                    await BluetoothPermissions.BackgroundLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
                }
                catch
                {
                    // Background location is optional, continue without it
                }

                return true;
            }

            // For older versions, request COARSE_LOCATION
            await BluetoothPermissions.CoarseLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> RequestBroadcasterPermissionsAsync()
    {
        try
        {
            await BluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

            // For API 31+ (Android 12+), request BLUETOOTH_ADVERTISE
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                await BluetoothPermissions.BluetoothAdvertisePermission.RequestIfNeededAsync().ConfigureAwait(false);
                return true;
            }

            // For older versions, no special permissions needed
            return true;
        }
        catch
        {
            return false;
        }
    }
}