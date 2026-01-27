using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothScanner : BaseBluetoothScanner, ScanCallbackProxy.IScanner
{
    /// <summary>
    /// Gets the BluetoothLeScanner instance.
    /// </summary>
    public BluetoothLeScanner BluetoothLeScanner => ((BluetoothAdapter)Adapter).NativeBluetoothAdapter.BluetoothLeScanner ?? throw new InvalidOperationException("BluetoothAdapter.BluetoothLeScanner is not available");

    /// <summary>
    /// Gets the scan callback proxy that handles scan events.
    /// </summary>
    public ScanCallbackProxy ScanCallbackProxy { get; }

    public BluetoothScanner(IBluetoothAdapter adapter, IBluetoothCharacteristicAccessServicesRepository characteristicAccessServicesRepository) : base(adapter, characteristicAccessServicesRepository)
    {
        // Callbacks
        ScanCallbackProxy = new ScanCallbackProxy(this);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Requests necessary Bluetooth permissions based on the Android API level:
    /// <list type="bullet">
    /// <item>API 31+: BLUETOOTH_SCAN, BLUETOOTH_CONNECT</item>
    /// <item>API 29-30: ACCESS_FINE_LOCATION, ACCESS_BACKGROUND_LOCATION</item>
    /// <item>Below API 29: ACCESS_COARSE_LOCATION</item>
    /// </list>
    /// </remarks>
    public async override Task EnsurePermissionsAsync(CancellationToken cancellationToken = default)
    {
        await BluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            await BluetoothPermissions.BluetoothScanPermission.RequestIfNeededAsync().ConfigureAwait(false);
            await BluetoothPermissions.BluetoothConnectPermission.RequestIfNeededAsync().ConfigureAwait(false);
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

    #region Advertisement Filtering

    /// <summary>
    /// Gets or sets the filter function to determine which native advertisements to process.
    /// By default, all advertisements are accepted.
    /// </summary>
    public Func<ScanCallbackType, ScanResult, bool> NativeAdvertisementFilter { get; set; } = DefaultNativeAdvertisementFilter;

    private static bool DefaultNativeAdvertisementFilter(ScanCallbackType callbackType, ScanResult result)
    {
        return true;
    }

    #endregion
}
