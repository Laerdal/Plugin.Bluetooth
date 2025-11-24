using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
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

    protected override void NativeRefreshIsBluetoothOn()
    {
        IsBluetoothOn = BluetoothAdapterProxy.BluetoothAdapter.IsEnabled;
    }
}
