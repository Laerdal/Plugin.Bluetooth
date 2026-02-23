using Bluetooth.Abstractions.Options;
using Bluetooth.Core.Infrastructure.Retries;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Permissions;
using Bluetooth.Maui.Platforms.Droid.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

using ScanMode = Android.Bluetooth.LE.ScanMode;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc cref="BaseBluetoothScanner" />
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
    private AutoResetEvent InternalAndroidStartScanResultReceived { get; } = new AutoResetEvent(false);

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

        var retryOptions = scanningOptions.ScanStartRetry ?? RetryOptions.None;

        if (retryOptions.MaxRetries > 0)
        {
            // Use retry logic for scan start
            await RetryTools.RunWithRetriesAsync(
                () => StartScanInternal(scanningOptions, cancellationToken),
                retryOptions,
                cancellationToken
            ).ConfigureAwait(false);
        }
        else
        {
            // No retries, single attempt
            await StartScanInternal(scanningOptions, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Internal method for starting the BLE scan. Extracted for retry logic.
    /// </summary>
    private async Task StartScanInternal(ScanningOptions scanningOptions, CancellationToken cancellationToken)
    {
        // Reset the event before starting scan
        InternalAndroidStartScanResultReceived.Reset();

        // Build scan settings and filters
        var settings = BuildScanSettings(scanningOptions);
        var filters = BuildScanFilters(scanningOptions);

        // Start the scan
        BluetoothLeScanner?.StartScan(filters, settings, ScanCallbackProxy);

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
            IsRunning = true;
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

        // Map abstract ScanMode to Android ScanMode
        var scanMode = options.ScanMode switch
        {
            BluetoothScanMode.LowPower => ScanMode.LowPower,
            BluetoothScanMode.Balanced => ScanMode.Balanced,
            BluetoothScanMode.LowLatency => ScanMode.LowLatency,
            BluetoothScanMode.Opportunistic when OperatingSystem.IsAndroidVersionAtLeast(24) => ScanMode.Opportunistic,
            BluetoothScanMode.Opportunistic => ScanMode.Balanced, // Fallback for API < 24
            _ => ScanMode.Balanced
        };
        builder.SetScanMode(scanMode);

        // Map abstract CallbackType to Android ScanCallbackType (API 23+)
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            var callbackType = options.CallbackType switch
            {
                BluetoothScanCallbackType.AllMatches => ScanCallbackType.AllMatches,
                BluetoothScanCallbackType.FirstMatch => ScanCallbackType.FirstMatch,
                BluetoothScanCallbackType.MatchLost => ScanCallbackType.MatchLost,
                BluetoothScanCallbackType.FirstMatchAndMatchLost => ScanCallbackType.FirstMatch | ScanCallbackType.MatchLost,
                _ => ScanCallbackType.AllMatches
            };
            builder.SetCallbackType(callbackType);
        }

        // Apply Android-specific options if provided
        if (options.Android is Options.AndroidScanningOptions androidOptions)
        {
            // Match mode (API 23+)
            if (OperatingSystem.IsAndroidVersionAtLeast(23) && androidOptions.MatchMode.HasValue)
            {
                var matchModeValue = androidOptions.MatchMode.Value switch
                {
                    Options.ScanMatchMode.Aggressive => 1, // MATCH_MODE_AGGRESSIVE
                    Options.ScanMatchMode.Sticky => 2,     // MATCH_MODE_STICKY
                    _ => 1
                };
                builder.SetMatchMode((Android.Bluetooth.LE.BluetoothScanMatchMode)matchModeValue);
            }

            // Number of matches (API 23+)
            if (OperatingSystem.IsAndroidVersionAtLeast(23) && androidOptions.NumOfMatches.HasValue)
            {
                var numMatchesValue = androidOptions.NumOfMatches.Value switch
                {
                    Options.ScanMatchCount.OneAdvertisement => 1,   // MATCH_NUM_ONE_ADVERTISEMENT
                    Options.ScanMatchCount.FewAdvertisements => 2,  // MATCH_NUM_FEW_ADVERTISEMENT
                    Options.ScanMatchCount.MaxAdvertisements => 3,  // MATCH_NUM_MAX_ADVERTISEMENT
                    _ => 1
                };
                builder.SetNumOfMatches(numMatchesValue);
            }

            // Report delay (API 23+)
            if (OperatingSystem.IsAndroidVersionAtLeast(23) && androidOptions.ReportDelay.HasValue)
            {
                builder.SetReportDelay((long)androidOptions.ReportDelay.Value.TotalMilliseconds);
            }

            // PHY (API 26+)
            if (OperatingSystem.IsAndroidVersionAtLeast(26) && androidOptions.Phy.HasValue)
            {
                // Use PhyModeConverter to convert ScanPhy to Android.Bluetooth.BluetoothPhy
                var phy = androidOptions.Phy.Value.ToAndroidBluetoothPhy();
                builder.SetPhy(phy);
            }

            // Legacy (API 26+)
            if (OperatingSystem.IsAndroidVersionAtLeast(26) && androidOptions.Legacy.HasValue)
            {
                builder.SetLegacy(androidOptions.Legacy.Value);
            }
        }

        return builder.Build() ?? throw new InvalidOperationException("Failed to build ScanSettings");
    }

    /// <summary>
    ///     Builds Android scan filters from the scanner options.
    /// </summary>
    private static IList<ScanFilter>? BuildScanFilters(ScanningOptions options)
    {
        // Return null if no service UUIDs specified (no filters needed)
        if (options.ServiceUuids == null || options.ServiceUuids.Count == 0)
        {
            return null;
        }

        // Build a scan filter for each service UUID
        var filters = new List<ScanFilter>(options.ServiceUuids.Count);
        foreach (var serviceUuid in options.ServiceUuids)
        {
            using var builder = new ScanFilter.Builder();

            // Convert Guid to ParcelUuid (Android UUID wrapper)
            var javaUuid = Java.Util.UUID.FromString(serviceUuid.ToString());
            if (javaUuid != null)
            {
                var parcelUuid = new ParcelUuid(javaUuid);
                builder.SetServiceUuid(parcelUuid);

                var filter = builder.Build();
                if (filter != null)
                {
                    filters.Add(filter);
                }
            }
        }

        return filters.Count > 0 ? filters : null;
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
