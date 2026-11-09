using Bluetooth.Abstractions.Options;
using Bluetooth.Core.Infrastructure.Retries;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Logging;
using Bluetooth.Maui.Platforms.Droid.Permissions;
using Bluetooth.Maui.Platforms.Droid.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc cref="BaseBluetoothScanner" />
public class AndroidBluetoothScanner : BaseBluetoothScanner, ScanCallbackProxy.IScanCallbackProxyDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothScanner" /> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this scanner.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="deviceFactory">The factory for creating platform-specific remote device instances.</param>
    /// <param name="nameProvider">Optional provider for Bluetooth device names.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    public AndroidBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ITicker ticker,
        IBluetoothRemoteDeviceFactory deviceFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null) : base(adapter,
                                                     rssiToSignalStrengthConverter,
                                                     ticker,
                                                     nameProvider,
                                                     loggerFactory)
    {
        _deviceFactory = deviceFactory;
        ScanCallbackProxy = new ScanCallbackProxy(this);
    }

    private readonly IBluetoothRemoteDeviceFactory _deviceFactory;

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

    private const int StartPending = 1; // a start is waiting for its first result or failure
    private const int StartClaimed = 2; // a result came first, or the wait ran out: the start is over
    private const int StartAbandoned = 3; // the caller gave up first and the scan is being stopped
    private const int StartFailed = 4; // a scan failure came first: the start throws it
    private int _startState;
    private AndroidNativeScanFailureException? _startFailure;

    // Held while a start is opened, and while a scan failure claims the start and signals it: a failure that claimed an attempt always has its
    // signal sent before the next attempt resets the event, so a signal can never wake an attempt that it does not belong to.
    private readonly object _startGate = new();

    /// <summary>
    ///     How long a start waits for a scan result or a failure before it reports the scan as started. Defaults to 2 seconds.
    /// </summary>
    /// <remarks>
    ///     Android gives no other feedback on a start, so when no advertisement passes the filters the wait runs out and the scan counts as started. A shorter
    ///     timeout passed to the start ends the wait earlier, and then running out of time is a failure.
    /// </remarks>
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

        // BluetoothLeScanner.StartScan() can succeed (no onScanFailed callback) even when the
        // adapter isn't really enabled - Android's internal BLE_ON low-power state (opportunistic
        // system-level scanning while the user's own Bluetooth toggle is off) accepts the call but
        // never delivers advertisements. Without this check that's indistinguishable from "started
        // fine, nothing nearby". Mirrors the CBManagerState.PoweredOn check in
        // AppleBluetoothScanner.NativeStartAsync.
        if (!Adapter.IsEnabled)
        {
            throw new ScannerFailedToStartException(this, "Failed to start scanning because Bluetooth is not enabled.");
        }

        Logger?.LogScanStarting();
        // TODO: Add LogScanStarting overload that accepts ScanMode and CallbackType parameters

        // Built once, before any attempt: an invalid option is the caller's mistake and a retry cannot fix it.
        var filters = BuildScanFilters(scanningOptions);

        var retryOptions = scanningOptions.ScanStartRetry ?? RetryOptions.None;

        if (retryOptions.MaxRetries > 0)
        {
            // Use retry logic for scan start
            var attempt = 0;
            TimeoutException? timedOut = null;
            try
            {
                await RetryTools.RunWithRetriesAsync(async () => {
                                                         attempt++;
                                                         try
                                                         {
                                                             await StartScanInternal(scanningOptions, filters, timeout, cancellationToken).ConfigureAwait(false);
                                                         }
                                                         catch (TimeoutException e)
                                                         {
                                                             // The caller's time is spent: another attempt would start the scan again and wait for as long.
                                                             timedOut = e;
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
                                                     cancellationToken)
                                .ConfigureAwait(false);

                if (timedOut is not null)
                {
                    Logger?.LogScanStartFailed(attempt, timedOut);
                    throw timedOut;
                }

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
                await StartScanInternal(scanningOptions, filters, timeout, cancellationToken).ConfigureAwait(false);
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
    private async Task StartScanInternal(ScanningOptions scanningOptions, List<ScanFilter>? filters, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        // Reset the event and open the start before starting scan
        lock (_startGate)
        {
            InternalAndroidStartScanResultReceived.Reset();
            _startFailure = null;
            Volatile.Write(ref _startState, StartPending);
        }

        // Build scan settings
        var settings = BuildScanSettings(scanningOptions);

        // Start the scan
        BluetoothLeScanner?.StartScan(filters, settings, ScanCallbackProxy);

        // Wait for a scan result, a scan failure or the caller giving up. On Android a result and a failure are the only feedback a start
        // gives: when no advertisement passes the filters (no device around, or none that matches) neither comes, so running out of
        // ScannerStartedTimeout means the scan started and found nothing. A caller who allows less time than that decides sooner, and
        // then running out of time is a failure. The cancellation is part of the wait, so it ends the wait at once.
        var callerDecides = timeout is { } allowed && allowed >= TimeSpan.Zero && allowed < ScannerStartedTimeout;
        var wait = callerDecides ? timeout!.Value : ScannerStartedTimeout;
        var waited = await Task.Run(() => WaitHandle.WaitAny([InternalAndroidStartScanResultReceived, cancellationToken.WaitHandle], wait))
                               .ConfigureAwait(false);

        // The start is decided once, by whoever claims it first: a result or a failure (OnScanResult, OnScanFailed), or the caller giving up
        // here. A start that a result already completed stands, and a cancellation or a timeout that comes after is too late, as for any call
        // that has completed: throwing then would reach the base class after the start succeeded, where it cannot be stored as its failure.
        // A start the caller gave up is not revived by a result that was already on its way (OnScanResult drops it).
        var cancelled = cancellationToken.IsCancellationRequested;
        var timedOut = waited == WaitHandle.WaitTimeout && callerDecides;
        if ((cancelled || timedOut) && Interlocked.CompareExchange(ref _startState, StartAbandoned, StartPending) == StartPending)
        {
            // The caller gave up first: no start is reported and the native scan does not keep running behind it.
            BluetoothLeScanner?.StopScan(ScanCallbackProxy);
            IsRunning = false;
            cancellationToken.ThrowIfCancellationRequested();
            throw new TimeoutException($"No scan result or failure came within the {timeout!.Value.TotalMilliseconds:0} ms given to start the scan.");
        }

        // Nothing came within ScannerStartedTimeout: the scan started and found nothing. Only when this wins the start: a result or a failure
        // that claimed it meanwhile has set IsRunning itself, and a failure must not be overwritten with a success.
        if (waited == WaitHandle.WaitTimeout && Interlocked.CompareExchange(ref _startState, StartClaimed, StartPending) == StartPending)
        {
            IsRunning = true;
        }

        // A scan failure that claimed the start fails it here, where the base class stores it as the failure of the start (and the start
        // retries can run), instead of the failure being thrown inside the native callback. The scan that failed is stopped so that a retry
        // starts clean.
        if (Volatile.Read(ref _startState) == StartFailed)
        {
            BluetoothLeScanner?.StopScan(ScanCallbackProxy);
            throw _startFailure!;
        }
    }

    /// <summary>
    ///     Claims the start for a scan result, or for the scan failure that says the scan is already started.
    /// </summary>
    /// <returns>
    ///     <see langword="false" /> when the start is no longer to be reported to: the caller gave up on it (the scan is being stopped), or a
    ///     scan failure already failed it.
    /// </returns>
    private bool ClaimStart()
    {
        return Interlocked.CompareExchange(ref _startState, StartClaimed, StartPending) is not (StartAbandoned or StartFailed);
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

        // Android's BluetoothLeScanner.stopScan() has no completion callback - confirmed via a
        // real-hardware hang: StopScanningAsync()'s internal wait for OnStopSucceeded() (which only
        // fires from the IsRunning setter, via OnIsRunningChanged) never completed, because nothing
        // in this class ever set IsRunning = false. Mirrors AppleBluetoothScanner.ScanningStopped(),
        // which does the same for the identical reason (CoreBluetooth's stopScan() is equally
        // synchronous/immediate with no confirmation delegate).
        IsRunning = false;

        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Builds Android scan settings from the scanner options.
    /// </summary>
    private static ScanSettings BuildScanSettings(ScanningOptions options)
    {
        using var builder = new ScanSettings.Builder();

        var scanMode = options.ScanMode.ToAndroidScanMode();
        builder.SetScanMode(scanMode);

        // Map abstract CallbackType to Android ScanCallbackType (API 23+)
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            var callbackType = options.CallbackType.ToAndroidScanCallbackType();
            builder.SetCallbackType(callbackType);
        }

        // Apply Android-specific options if provided
        if (options.Android is Abstractions.Scanning.Options.Android.AndroidScanningOptions androidOptions)
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
                builder.SetNumOfMatches((int) numMatches);
            }

            // Report delay (API 23+)
            if (OperatingSystem.IsAndroidVersionAtLeast(23) && androidOptions.ReportDelay.HasValue)
            {
                builder.SetReportDelay((long) androidOptions.ReportDelay.Value.TotalMilliseconds);
            }

            // PHY (API 26+)
            if (OperatingSystem.IsAndroidVersionAtLeast(26) && androidOptions.Phy.HasValue)
            {
                builder.SetPhy(androidOptions.Phy.Value.ToAndroidScanPhy());
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
        var manufacturerIds = (options.Android as Abstractions.Scanning.Options.Android.AndroidScanningOptions)?.ManufacturerIds;
        var plan = AndroidScanFilterPlanner.Plan(options.ServiceUuids, manufacturerIds);

        // Return null if there is nothing to filter on (no filters needed)
        if (plan.Count == 0)
        {
            return null;
        }

        var filters = new List<ScanFilter>(plan.Count);
        foreach (var spec in plan)
        {
            using var builder = new ScanFilter.Builder();

            if (spec.ServiceUuid is { } serviceUuid)
            {
                // Convert Guid to ParcelUuid (Android UUID wrapper)
                var javaUuid = Java.Util.UUID.FromString(serviceUuid.ToString());
                if (javaUuid == null)
                {
                    continue;
                }

                using var parcelUuid = new ParcelUuid(javaUuid);
                builder.SetServiceUuid(parcelUuid);
            }

            if (spec.ManufacturerId is { } manufacturerId)
            {
                // The company identifier alone: with no data to compare, any manufacturer data of that company matches.
                builder.SetManufacturerData(manufacturerId, []);
            }

            var filter = builder.Build();
            if (filter != null)
            {
                filters.Add(filter);
            }
        }

        return filters.Count > 0 ? filters : null;
    }

    /// <inheritdoc />
    protected override IBluetoothRemoteDevice NativeCreateDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        var spec = new AndroidBluetoothRemoteDeviceFactorySpec(advertisement);
        return _deviceFactory.Create(this, spec);
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
            if (ClaimStart())
            {
                IsRunning = true;
                InternalAndroidStartScanResultReceived.Set();
            }
        }
        else
        {
            var failure = new AndroidNativeScanFailureException(errorCode);

            int previous;
            lock (_startGate)
            {
                _startFailure = failure; // before the claim: the start reads it once it sees the state
                previous = Interlocked.CompareExchange(ref _startState, StartFailed, StartPending);
                if (previous == StartPending)
                {
                    // The failure decides the start: it wakes the start, which throws it. IsRunning is already false while a start is
                    // pending, and is left alone here: a write after this point could land on the next attempt.
                    InternalAndroidStartScanResultReceived.Set();
                    return;
                }
            }

            if (previous == StartAbandoned)
            {
                // The caller gave up on the start: nobody is waiting for the failure.
                return;
            }

            // The start was already reported, or none was pending: only the unhandled exception listener can take the failure.
            IsRunning = false;
            throw failure;
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
        // Signal that scan has started successfully, unless the caller already gave up on the start
        if (!ClaimStart())
        {
            return;
        }

        IsRunning = true;
        InternalAndroidStartScanResultReceived.Set();

        ArgumentNullException.ThrowIfNull(result);
        var nativeDevice = result.Device;
        if (nativeDevice == null)
        {
            return;
        }

        var advertisement = new AndroidBluetoothAdvertisement(result);
        var deviceName = string.IsNullOrEmpty(advertisement.DeviceName) ? "(no name)" : advertisement.DeviceName;
        Logger?.LogDeviceDiscovered(nativeDevice.Address ?? "Unknown", deviceName, advertisement.Manufacturer, result.Rssi);

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
        cancellationToken.ThrowIfCancellationRequested();

        // For API 31+ (Android 12+), spec BLUETOOTH_SCAN only (not CONNECT)
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            await AndroidBluetoothPermissions.BluetoothScanPermission.RequestIfNeededAsync().ConfigureAwait(false);
            return;
        }

        // Legacy BLUETOOTH permission only applies below API 31 - on API 31+ it's superseded by
        // BLUETOOTH_SCAN/CONNECT, and the OS elides it from the installed package's reported permission
        // list for apps targeting SDK 31+. Requesting it unconditionally makes
        // Permissions.BasePlatformPermission.CheckStatusAsync() falsely report it as "not declared in
        // AndroidManifest.xml" even when it is, crashing every scan attempt on modern Android.
        await AndroidBluetoothPermissions.BluetoothPermission.RequestIfNeededAsync().ConfigureAwait(false);

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
