using Bluetooth.Abstractions.Options;

namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents a Bluetooth scanner configuration.
/// </summary>
public record ScanningOptions
{
    #region Permission Handling

    /// <summary>
    ///     Gets the permission request strategy for this scanning operation.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see cref="PermissionRequestStrategy.RequestAutomatically"/> which automatically
    ///     requests permissions before starting the scan if not already granted.
    /// </remarks>
    public PermissionRequestStrategy PermissionStrategy { get; init; } = PermissionRequestStrategy.RequestAutomatically;

    /// <summary>
    ///     Gets a value indicating whether background location permission should be requested on Android.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: API 29-30 (Android 10-11) requests ACCESS_BACKGROUND_LOCATION if true; required for background scanning</item>
    ///         <item><b>iOS/macOS</b>: Ignored (background permissions handled by Info.plist)</item>
    ///         <item><b>Windows</b>: Ignored (no background permission needed)</item>
    ///     </list>
    ///     Defaults to false (foreground scanning only).
    /// </remarks>
    public bool RequireBackgroundLocation { get; init; }

    #endregion

    /// <summary>
    ///     Gets a value indicating whether duplicate advertisements should be ignored.
    /// </summary>
    public bool IgnoreDuplicateAdvertisements { get; init; }

    /// <summary>
    ///     Gets a value indicating whether advertisements without a local name should be ignored.
    /// </summary>
    public bool IgnoreNamelessAdvertisements { get; init; }

    /// <summary>
    ///     Advertisement filter. If set, only advertisements that pass the filter will be processed.
    /// </summary>
    public Func<IBluetoothAdvertisement, bool> AdvertisementFilter { get; init; } = _ => true;

    #region Service UUID Filtering

    /// <summary>
    ///     Gets the list of service UUIDs to filter during scanning.
    ///     When set, only devices advertising these services will be discovered.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Full support via ScanFilter</item>
    ///         <item><b>iOS/macOS</b>: Full support via CBCentralManager scanForPeripherals serviceUUIDs parameter</item>
    ///         <item><b>Windows</b>: Filtering performed in software after advertisement received</item>
    ///     </list>
    /// </remarks>
    public IReadOnlyList<Guid>? ServiceUuids { get; init; }

    #endregion

    #region Extended Advertising Support (Bluetooth 5.0+)

    /// <summary>
    ///     Gets a value indicating whether to enable extended advertising support (Bluetooth 5.0+).
    ///     Extended advertising provides larger payloads and additional features.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Supported on Android 8.0 (API 26+) with Bluetooth 5.0 hardware</item>
    ///         <item><b>iOS/macOS</b>: Automatically supported on devices with Bluetooth 5.0</item>
    ///         <item><b>Windows</b>: Supported on Windows 10 version 2004+ with Bluetooth 5.0 adapter</item>
    ///     </list>
    /// </remarks>
    public bool EnableExtendedAdvertising { get; init; }

    #endregion

    #region Signal Strength Options

    /// <summary>
    ///     Gets the options for smoothing signal strength jitter.
    /// </summary>
    public SignalStrengthSmoothingOptions SignalStrengthJitterSmoothing { get; init; } = new SignalStrengthSmoothingOptions();

    #endregion

    #region Retry Configuration

    /// <summary>
    ///     Gets the retry configuration for scan start operations.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Retry configuration applied when starting the BLE scanner fails due to transient issues
    ///         such as adapter busy, scan already started, or throttling errors.
    ///     </para>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Retries on ScanFailure errors (except AlreadyStarted)</item>
    ///         <item><b>iOS/macOS</b>: Retries on CBCentralManager state issues</item>
    ///         <item><b>Windows</b>: Retries on scanner start failures</item>
    ///     </list>
    ///     <para>
    ///         Defaults to <see cref="RetryOptions.Default"/> (3 retries with 200ms delay).
    ///         Set to <see cref="RetryOptions.None"/> to disable retry logic.
    ///     </para>
    /// </remarks>
    public RetryOptions? ScanStartRetry { get; init; } = RetryOptions.Default;

    #endregion

    #region Scan Mode / Power Settings

    /// <summary>
    ///     Gets the scan mode that controls power consumption and scan latency.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Full support via ScanSettings.ScanMode</item>
    ///         <item><b>iOS/macOS</b>: Mapped to CBCentralManagerScanOptions (allowDuplicatesKey affects behavior)</item>
    ///         <item><b>Windows</b>: Mapped to sampling interval and signal strength filter settings</item>
    ///     </list>
    ///     <para>
    ///         <see cref="BluetoothScanMode.LowPower" />: Optimizes for battery life (infrequent scans)
    ///     </para>
    ///     <para>
    ///         <see cref="BluetoothScanMode.Balanced" />: Balanced power and latency (default)
    ///     </para>
    ///     <para>
    ///         <see cref="BluetoothScanMode.LowLatency" />: Optimizes for fast discovery (frequent scans, higher power consumption)
    ///     </para>
    /// </remarks>
    public BluetoothScanMode ScanMode { get; init; } = BluetoothScanMode.Balanced;

    /// <summary>
    ///     Gets the callback type that controls when scan results are reported.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Full support via ScanSettings.CallbackType</item>
    ///         <item><b>iOS/macOS</b>: All callbacks reported immediately (always FirstMatch behavior)</item>
    ///         <item><b>Windows</b>: All callbacks reported immediately (always FirstMatch behavior)</item>
    ///     </list>
    /// </remarks>
    public BluetoothScanCallbackType CallbackType { get; init; } = BluetoothScanCallbackType.AllMatches;

    /// <summary>
    ///     Gets the RSSI threshold in dBm for filtering scan results.
    ///     Devices with RSSI below this threshold will be filtered out.
    ///     Only applicable when filtering by signal strength.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Not directly supported, filtering performed in software</item>
    ///         <item><b>iOS/macOS</b>: Not directly supported, filtering performed in software</item>
    ///         <item><b>Windows</b>: Full support via BluetoothLEAdvertisementWatcher.SignalStrengthFilter</item>
    ///     </list>
    ///     <para>Typical values: -100 (very weak) to -30 (very strong)</para>
    /// </remarks>
    public int? RssiThreshold { get; init; }

    #endregion

    #region Platform-Specific Scanning Options

    /// <summary>
    ///     Gets the Android platform-specific scanning options.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Should be an instance of <c>Bluetooth.Maui.Platforms.Droid.Scanning.Options.AndroidScanningOptions</c>.
    ///         These options are only used on Android platforms and are ignored on other platforms.
    ///     </para>
    ///     <para>
    ///         Provides access to Android-specific scan settings such as:
    ///     </para>
    ///     <list type="bullet">
    ///         <item><b>MatchMode</b>: Control aggressive vs sticky matching (API 23+)</item>
    ///         <item><b>NumOfMatches</b>: Set advertisement match count before reporting (API 23+)</item>
    ///         <item><b>ReportDelay</b>: Batch scan results for power savings (API 23+)</item>
    ///         <item><b>Phy</b>: Select Bluetooth 5.0 PHY layers (API 26+)</item>
    ///         <item><b>Legacy</b>: Filter for legacy-only advertisements (API 26+)</item>
    ///     </list>
    /// </remarks>
    public object? Android { get; init; }

    #endregion
}

/// <summary>
///     Defines scan modes that control the power consumption and scan latency trade-off.
/// </summary>
public enum BluetoothScanMode
{
    /// <summary>
    ///     Low power mode with infrequent scans. Optimizes for battery life.
    /// </summary>
    /// <remarks>
    ///     Android: SCAN_MODE_LOW_POWER (scan interval ~5 seconds)
    /// </remarks>
    LowPower = 0,

    /// <summary>
    ///     Balanced mode providing a compromise between power consumption and scan latency.
    /// </summary>
    /// <remarks>
    ///     Android: SCAN_MODE_BALANCED (scan interval ~2 seconds)
    /// </remarks>
    Balanced = 1,

    /// <summary>
    ///     Low latency mode with frequent scans. Optimizes for fast discovery at the cost of higher power consumption.
    /// </summary>
    /// <remarks>
    ///     Android: SCAN_MODE_LOW_LATENCY (scan continuously or near-continuously)
    /// </remarks>
    LowLatency = 2,

    /// <summary>
    ///     Opportunistic mode where scans are performed only when other apps are scanning.
    ///     Provides best battery life but unpredictable scan frequency.
    /// </summary>
    /// <remarks>
    ///     Android: SCAN_MODE_OPPORTUNISTIC (Android 7.0+)
    ///     Not supported on iOS/Windows (falls back to Balanced)
    /// </remarks>
    Opportunistic = -1
}

/// <summary>
///     Defines when scan result callbacks should be triggered.
/// </summary>
[Flags]
public enum BluetoothScanCallbackType
{
    /// <summary>
    ///     Do not report any advertisement packets. This can be used when only interested in scan start/stop events or when filtering is performed in software.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Report all advertisement packets as they are received.
    /// </summary>
    /// <remarks>
    ///     Android: CALLBACK_TYPE_ALL_MATCHES
    /// </remarks>
    AllMatches = 1,

    /// <summary>
    ///     Report only the first advertisement packet from each device.
    /// </summary>
    /// <remarks>
    ///     Android: CALLBACK_TYPE_FIRST_MATCH
    /// </remarks>
    FirstMatch = 2,

    /// <summary>
    ///     Report when a device is no longer detected (after a timeout period).
    /// </summary>
    /// <remarks>
    ///     Android: CALLBACK_TYPE_MATCH_LOST
    ///     Not fully supported on iOS/Windows
    /// </remarks>
    MatchLost = 4,

    /// <summary>
    ///     Report both first match and match lost events.
    /// </summary>
    /// <remarks>
    ///     Android: CALLBACK_TYPE_FIRST_MATCH | CALLBACK_TYPE_MATCH_LOST
    /// </remarks>
    FirstMatchAndMatchLost = FirstMatch | MatchLost
}
