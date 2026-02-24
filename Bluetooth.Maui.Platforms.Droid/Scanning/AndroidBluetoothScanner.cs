using Bluetooth.Abstractions.Options;
using Bluetooth.Core.Infrastructure.Retries;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Logging;
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
        IBluetoothRemoteDeviceFactory deviceFactory,
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
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/le/BluetoothLeScanner">Android BluetoothLeScanner</seealso>
    protected async override ValueTask NativeStartAsync(ScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothLeScanner);
        ArgumentNullException.ThrowIfNull(scanningOptions);

        Logger?.LogScanStarting(scanningOptions.ScanMode, scanningOptions.CallbackType);

        var retryOptions = scanningOptions.ScanStartRetry ?? RetryOptions.None;

        if (retryOptions.MaxRetries > 0)
        {
            // Use retry logic for scan start
            var attempt = 0;
            try
            {
                await RetryTools.RunWithRetriesAsync(
                    async () =>
                    {
                        attempt++;
                        try
                        {
                            await StartScanInternal(scanningOptions, cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            if (attempt < retryOptions.MaxRetries)
                            {
                                Logger?.LogScanStartRetry(attempt, retryOptions.MaxRetries, ex);
                            }
                            throw;
                        }
                    },
                    retryOptions,
                    cancellationToken
                ).ConfigureAwait(false);

                Logger?.LogScanStarted();
            }
            catch (AggregateException ex)
            {
                Logger?.LogScanStartFailed(attempt, ex);
                throw;
            }
        }
        else
        {
            // No retries, single attempt
            try
            {
                await StartScanInternal(scanningOptions, cancellationToken).ConfigureAwait(false);
                Logger?.LogScanStarted();
            }
            catch (Exception ex)
            {
                Logger?.LogScanStartFailed(1, ex);
                throw;
            }
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
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/le/BluetoothLeScanner">Android BluetoothLeScanner</seealso>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothLeScanner);
        Logger?.LogScanStopping();
        BluetoothLeScanner.StopScan(ScanCallbackProxy);
        Logger?.LogScanStopped();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Builds Android scan settings from the scanner options.
    /// </summary>
    private static ScanSettings BuildScanSettings(ScanningOptions options)
    {
        using var builder = new ScanSettings.Builder();

        // Map abstract ScanMode to Android ScanMode using converter
        var scanMode = options.ScanMode.ToAndroidScanMode();
        builder.SetScanMode(scanMode);

        // Map abstract CallbackType to Android ScanCallbackType (API 23+)
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            var callbackType = options.CallbackType.ToAndroidScanCallbackType();
            builder.SetCallbackType(callbackType);
        }

        // Apply Android-specific options if provided
        if (options.Android is Options.AndroidScanningOptions androidOptions)
        {
            // Match mode (API 23+)
            if (OperatingSystem.IsAndroidVersionAtLeast(23) && androidOptions.MatchMode.HasValue)
            {
                var matchMode = androidOptions.MatchMode.Value.ToAndroidMatchMode();
                builder.SetMatchMode(matchMode);
            }

            // Number of matches (API 23+)
            if (OperatingSystem.IsAndroidVersionAtLeast(23) && androidOptions.ScanMatchNumber.HasValue)
            {
                var numMatches = androidOptions.ScanMatchNumber.Value.ToAndroidNumOfMatches();
                builder.SetNumOfMatches((int)numMatches);
            }

            // Report delay (API 23+)
            if (OperatingSystem.IsAndroidVersionAtLeast(23) && androidOptions.ReportDelay.HasValue)
            {
                builder.SetReportDelay((long)androidOptions.ReportDelay.Value.TotalMilliseconds);
            }

            // PHY (API 26+)
            if (OperatingSystem.IsAndroidVersionAtLeast(26) && androidOptions.Phy.HasValue)
            {
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
    private static List<ScanFilter>? BuildScanFilters(ScanningOptions options)
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
                using var parcelUuid = new ParcelUuid(javaUuid);
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
    protected override IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec CreateDeviceFactoryRequestFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        return new AndroidBluetoothRemoteDeviceFactorySpec(advertisement);
    }

    #region ScanCallbackProxy.IScanCallbackProxyDelegate Implementation

    /// <summary>
    ///     Handles scan failures from the Android Bluetooth LE scanner.
    /// </summary>
    public void OnScanFailed(ScanFailure errorCode)
    {
        Logger?.LogScanFailure(errorCode.ToString());

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

        Logger?.LogDeviceDiscovered(nativeDevice.Address ?? "Unknown", result.Rssi);

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
    protected async override ValueTask<bool> NativeHasScannerPermissionsAsync()
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
    protected async override ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken)
    {
        await AndroidBluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

        // For API 31+ (Android 12+), spec BLUETOOTH_SCAN only (not CONNECT)
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            await AndroidBluetoothPermissions.BluetoothScanPermission.RequestIfNeededAsync().ConfigureAwait(false);
            return;
        }

        // For API 29-30 (Android 10-11), spec FINE_LOCATION and optionally BACKGROUND_LOCATION
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            await AndroidBluetoothPermissions.FineLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);

            // Optionally spec background location if specified
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

        // For older versions, spec COARSE_LOCATION
        await AndroidBluetoothPermissions.CoarseLocationPermission.RequestIfNeededAsync().ConfigureAwait(false);
    }

    #endregion
}
