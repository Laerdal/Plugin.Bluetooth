namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents a Bluetooth scanner configuration.
/// </summary>
public record ScanningOptions
{
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
    public SignalStrengthSmoothingOptions SignalStrengthJitterSmoothing { get; init; } = new();

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