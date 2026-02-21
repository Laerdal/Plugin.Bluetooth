using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Permissions;
using Bluetooth.Maui.Platforms.Droid.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

using ScanMode = Android.Bluetooth.LE.ScanMode;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc />
public class AndroidBluetoothScanner : BaseBluetoothScanner, ScanCallbackProxy.IScanCallbackProxyDelegate
{
    /// <inheritdoc />
    public AndroidBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothDeviceFactory deviceFactory,
        ITicker ticker,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothScanner>? logger = null) : base(adapter,
        deviceFactory,
        rssiToSignalStrengthConverter,
        ticker,
        logger)
    {
        ScanCallbackProxy = new ScanCallbackProxy(this);
    }

    /// <summary>
    ///     Gets the BluetoothLeScanner instance from the Android Bluetooth adapter.
    /// </summary>
    private BluetoothLeScanner BluetoothLeScanner
    {
        get
        {
            if (Adapter is not AndroidBluetoothAdapter androidAdapter)
            {
                throw new InvalidOperationException("Adapter must be an Android BluetoothAdapter");
            }

            return androidAdapter.NativeBluetoothAdapter.BluetoothLeScanner ?? throw new InvalidOperationException("BluetoothLeScanner is not available");
        }
    }

    /// <summary>
    ///     Gets the scan callback proxy that handles scan events.
    /// </summary>
    private ScanCallbackProxy ScanCallbackProxy { get; }

    /// <summary>
    ///     Event to signal when scan start result is received.
    /// </summary>
    private AutoResetEvent InternalAndroidStartScanResultReceived { get; } = new(false);

    /// <summary>
    ///     Timeout for waiting for scan to start. Defaults to 2 seconds.
    /// </summary>
    public static TimeSpan ScannerStartedTimeout { get; set; } = TimeSpan.FromSeconds(2);

    /// <inheritdoc />
    /// <remarks>
    ///     On Android, there is no direct way to check if BLE scanning is running.
    ///     This is tracked manually when starting/stopping the scan and when receiving advertisements.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        // On Android there is no way to check if Scan is running (IsDiscovering only refers to Classic Bluetooth)
        // Tracking of this is done manually when starting/stopping the scan and when receiving advertisements
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Starts BLE scanning using the Android BluetoothLeScanner.
    /// </remarks>
    protected async override ValueTask NativeStartAsync(ScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothLeScanner);
        ArgumentNullException.ThrowIfNull(scanningOptions);

        // Build scan settings and filters
        var settings = BuildScanSettings(scanningOptions);
        var filters = BuildScanFilters(scanningOptions);

        BluetoothLeScanner.StartScan(filters, settings, ScanCallbackProxy);

        try
        {
            // Wait for either scan to start or timeout
            await Task.Run(() => InternalAndroidStartScanResultReceived.WaitOne(ScannerStartedTimeout), cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            // On Android, we only get feedback on "StartScanFailed" and on "AdvertisementReceived"
            // If no devices are around and scan started successfully, we don't receive anything
            // In timeout case, assume scan started successfully
            SetValue(true, nameof(IsRunning));
        }
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Stops BLE scanning using the Android BluetoothLeScanner.
    /// </remarks>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothLeScanner);
        BluetoothLeScanner.StopScan(ScanCallbackProxy);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Builds Android scan settings from the scanner options.
    /// </summary>
    private static ScanSettings BuildScanSettings(ScanningOptions options)
    {
        using var builder = new ScanSettings.Builder();

        // Set scan mode for balanced power/performance
        builder.SetScanMode(ScanMode.Balanced);

        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            builder.SetCallbackType(ScanCallbackType.AllMatches);
        }

        return builder.Build() ?? throw new InvalidOperationException("Failed to build ScanSettings");
    }

    /// <summary>
    ///     Builds Android scan filters from the scanner options.
    /// </summary>
    private static IList<ScanFilter>? BuildScanFilters(ScanningOptions options)
    {
        // No service UUID filtering in base interface, so return null for now
        // This can be extended if needed
        return null;
    }

    /// <inheritdoc />
    protected override IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest CreateDeviceFactoryRequestFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        return new AndroidBluetoothDeviceFactoryRequest(advertisement);
    }

    #region ScanCallbackProxy.IScanCallbackProxyDelegate Implementation

    /// <summary>
    ///     Handles scan failures from the Android Bluetooth LE scanner.
    /// </summary>
    public void OnScanFailed(ScanFailure errorCode)
    {
        if (errorCode == ScanFailure.AlreadyStarted)
        {
            IsRunning = true;
            InternalAndroidStartScanResultReceived.Set();
        }
        else
        {
            IsRunning = false;
            InternalAndroidStartScanResultReceived.Set();
            throw new AndroidNativeScanFailureException(errorCode);
        }
    }

    /// <summary>
    ///     Handles batch scan results from the Android Bluetooth LE scanner.
    /// </summary>
    public void OnScanResult(ScanCallbackType callbackType, IEnumerable<ScanResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);
        foreach (var result in results)
        {
            OnScanResult(callbackType, result);
        }
    }

    /// <summary>
    ///     Handles individual scan results from the Android Bluetooth LE scanner.
    /// </summary>
    public void OnScanResult(ScanCallbackType callbackType, ScanResult result)
    {
        // Signal that scan has started successfully
        IsRunning = true;
        InternalAndroidStartScanResultReceived.Set();

        ArgumentNullException.ThrowIfNull(result);
        var nativeDevice = result.Device;
        if (nativeDevice == null)
        {
            return;
        }

        var advertisement = new AndroidBluetoothAdvertisement(result);

        // Use base class method to handle advertisement (filtering, device creation, etc.)
        OnAdvertisementReceived(advertisement);
    }

    #endregion

    #region Permission Methods

    /// <inheritdoc />
    /// <remarks>
    ///     On Android, scanner permissions vary by API level:
    ///     <list type="bullet">
    ///         <item>API 31+ (Android 12+): Requires BLUETOOTH_SCAN permission</item>
    ///         <item>API 29-30 (Android 10-11): Requires FINE_LOCATION permission</item>
    ///         <item>Older versions: Requires COARSE_LOCATION permission</item>
    ///     </list>
    /// </remarks>
    protected override async ValueTask<bool> NativeHasScannerPermissionsAsync()
    {
        try
        {
            // For API 31+ (Android 12+), need BLUETOOTH_SCAN only (not CONNECT)
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                var scanStatus = await AndroidBluetoothPermissions.BluetoothScanPermission.CheckStatusAsync().ConfigureAwait(false);
                return scanStatus == PermissionStatus.Granted;
            }

            // For API 29-30 (Android 10-11), need FINE_LOCATION
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                var status = await AndroidBluetoothPermissions.FineLocationPermission.CheckStatusAsync().ConfigureAwait(false);
                return status == PermissionStatus.Granted;
            }

            // For older versions, COARSE_LOCATION is sufficient
            var coarseStatus = await AndroidBluetoothPermissions.CoarseLocationPermission.CheckStatusAsync().ConfigureAwait(false);
            return coarseStatus == PermissionStatus.Granted;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Android, scanner permissions vary by API level:
    ///     <list type="bullet">
    ///         <item>API 31+ (Android 12+): Requests BLUETOOTH_SCAN permission</item>
    ///         <item>API 29-30 (Android 10-11): Requests FINE_LOCATION (and optionally BACKGROUND_LOCATION)</item>
    ///         <item>Older versions: Requests COARSE_LOCATION permission</item>
    ///     </list>
    ///     The <paramref name="requireBackgroundLocation"/> parameter only applies to API 29-30.
    /// </remarks>
    protected override async ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken)
    {
        await AndroidBluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

        // For API 31+ (Android 12+), request BLUETOOTH_SCAN only (not CONNECT)
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            await AndroidBluetoothPermissions.BluetoothScanPermission.RequestIfNeededAsync().ConfigureAwait(false);
            return;
        }

        // For API 29-30 (Android 10-11), request FINE_LOCATION and optionally BACKGROUND_LOCATION
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            await AndroidBluetoothPermissions.FineLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);

            // Optionally request background location if specified
            if (requireBackgroundLocation)
            {
                try
                {
                    await AndroidBluetoothPermissions.BackgroundLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
                }
                catch
                {
                    // Background location is optional, continue without it
                    // User can still scan in foreground
                }
            }

            return;
        }

        // For older versions, request COARSE_LOCATION
        await AndroidBluetoothPermissions.CoarseLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
    }

    #endregion
}