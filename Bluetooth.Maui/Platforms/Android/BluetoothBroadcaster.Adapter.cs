using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    /// <remarks>
    /// Requests necessary Bluetooth permissions based on the Android API level:
    /// <list type="bullet">
    /// <item>API 31+: BLUETOOTH_ADVERTISE</item>
    /// <item>API 29-30: ACCESS_FINE_LOCATION, ACCESS_BACKGROUND_LOCATION</item>
    /// <item>Below API 29: ACCESS_COARSE_LOCATION</item>
    /// </list>
    /// </remarks>
    protected async override ValueTask NativeInitializeAsync()
    {
        await BluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            await BluetoothPermissions.BluetoothAdvertisePermission.RequestIfNeededAsync().ConfigureAwait(false);
        }
        else if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            await BluetoothPermissions.FineLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);

            // For using Bluetooth LE in Background
            await BluetoothPermissions.BackgroundLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
        }
        else
        {
            await BluetoothPermissions.CoarseLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On Android, this checks if the Bluetooth adapter is enabled.
    /// </remarks>
    protected override void NativeRefreshIsBluetoothOn()
    {
        IsBluetoothOn = BluetoothAdapterProxy.BluetoothAdapter.IsEnabled;
    }
}
