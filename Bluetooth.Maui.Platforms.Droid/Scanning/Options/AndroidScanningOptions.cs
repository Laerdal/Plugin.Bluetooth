namespace Bluetooth.Maui.Platforms.Droid.Scanning.Options;

/// <summary>
///     Android platform-specific scanning options.
/// </summary>
/// <remarks>
///     These options map directly to Android's ScanSettings.Builder properties.
///     See: https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder
/// </remarks>
public record AndroidScanningOptions
{
    /// <summary>
    ///     Gets the match mode for Bluetooth LE scan filters (API 23+).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Controls how aggressively the Bluetooth stack filters scan results:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="ScanMatchMode.Aggressive"/>: Few matches per filter (fewer false positives, default)
    ///         </item>
    ///         <item>
    ///             <see cref="ScanMatchMode.Sticky"/>: More matches per filter (higher discovery rate)
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Requires Android 6.0 (API 23) or higher. Ignored on lower API levels.
    ///     </para>
    /// </remarks>
    public ScanMatchMode? MatchMode { get; init; }

    /// <summary>
    ///     Gets the number of advertisements to match per filter before reporting (API 23+).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Controls when scan results are reported:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="ScanMatchCount.OneAdvertisement"/>: Report after matching one advertisement (fastest discovery, default)
    ///         </item>
    ///         <item>
    ///             <see cref="ScanMatchCount.FewAdvertisements"/>: Report after matching a few advertisements
    ///         </item>
    ///         <item>
    ///             <see cref="ScanMatchCount.MaxAdvertisements"/>: Report after matching many advertisements (reduces callbacks)
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Requires Android 6.0 (API 23) or higher. Ignored on lower API levels.
    ///     </para>
    /// </remarks>
    public ScanMatchCount? NumOfMatches { get; init; }

    /// <summary>
    ///     Gets the delay before reporting scan results (API 23+).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Setting a delay allows batching of scan results, reducing power consumption.
    ///         A delay of <see cref="TimeSpan.Zero"/> reports results immediately (default).
    ///     </para>
    ///     <para>
    ///         Example: 5-second delay batches all discovered devices and reports them together
    ///         every 5 seconds, reducing callback frequency and improving battery life.
    ///     </para>
    ///     <para>
    ///         Requires Android 6.0 (API 23) or higher. Ignored on lower API levels.
    ///     </para>
    /// </remarks>
    public TimeSpan? ReportDelay { get; init; }

    /// <summary>
    ///     Gets the PHY (Physical Layer) to use for scanning (API 26+).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Controls which Bluetooth 5.0 PHY layers to scan on:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="ScanPhy.AllSupported"/>: All supported PHYs (default, best compatibility)
    ///         </item>
    ///         <item>
    ///             <see cref="ScanPhy.Le1M"/>: 1M PHY only (standard Bluetooth LE)
    ///         </item>
    ///         <item>
    ///             <see cref="ScanPhy.Le2M"/>: 2M PHY (higher throughput, Bluetooth 5.0+)
    ///         </item>
    ///         <item>
    ///             <see cref="ScanPhy.LeCoded"/>: Coded PHY (Long Range, Bluetooth 5.0+)
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Requires Android 8.0 (API 26) or higher and Bluetooth 5.0 hardware. Ignored on lower API levels or older hardware.
    ///     </para>
    /// </remarks>
    public ScanPhy? Phy { get; init; }

    /// <summary>
    ///     Gets whether to scan for legacy advertisements only (API 26+).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When <c>true</c>, only legacy (pre-Bluetooth 5.0) advertisements are reported.
    ///         When <c>false</c> (default), both legacy and extended advertisements are reported.
    ///     </para>
    ///     <para>
    ///         Requires Android 8.0 (API 26) or higher. Ignored on lower API levels.
    ///     </para>
    /// </remarks>
    public bool? Legacy { get; init; }
}

/// <summary>
///     Android scan match modes (API 23+).
/// </summary>
/// <remarks>
///     Maps to Android ScanSettings match mode constants.
/// </remarks>
public enum ScanMatchMode
{
    /// <summary>
    ///     Aggressive mode with few matches per filter (fewer false positives).
    ///     Maps to ScanSettings.MATCH_MODE_AGGRESSIVE (value: 1).
    /// </summary>
    Aggressive = 1,

    /// <summary>
    ///     Sticky mode with more matches per filter (higher discovery rate).
    ///     Maps to ScanSettings.MATCH_MODE_STICKY (value: 2).
    /// </summary>
    Sticky = 2
}

/// <summary>
///     Android scan match counts (API 23+).
/// </summary>
/// <remarks>
///     Maps to Android ScanSettings match count constants.
/// </remarks>
public enum ScanMatchCount
{
    /// <summary>
    ///     Report after matching one advertisement.
    ///     Maps to ScanSettings.MATCH_NUM_ONE_ADVERTISEMENT (value: 1).
    /// </summary>
    OneAdvertisement = 1,

    /// <summary>
    ///     Report after matching a few advertisements.
    ///     Maps to ScanSettings.MATCH_NUM_FEW_ADVERTISEMENT (value: 2).
    /// </summary>
    FewAdvertisements = 2,

    /// <summary>
    ///     Report after matching many advertisements.
    ///     Maps to ScanSettings.MATCH_NUM_MAX_ADVERTISEMENT (value: 3).
    /// </summary>
    MaxAdvertisements = 3
}

/// <summary>
///     Android PHY types for scanning (API 26+).
/// </summary>
/// <remarks>
///     Maps to Android ScanSettings PHY constants.
///     Note: This enum uses [Flags] because Android supports scanning on multiple PHYs simultaneously.
/// </remarks>
[Flags]
public enum ScanPhy
{
    /// <summary>
    ///     All supported PHYs (default).
    ///     Maps to ScanSettings.PHY_LE_ALL_SUPPORTED (value: 0).
    /// </summary>
    AllSupported = 0,

    /// <summary>
    ///     1M PHY only (standard Bluetooth LE).
    ///     Maps to ScanSettings.PHY_LE_1M_MASK (value: 1).
    /// </summary>
    Le1M = 1,

    /// <summary>
    ///     2M PHY (higher throughput, Bluetooth 5.0+).
    ///     Maps to ScanSettings.PHY_LE_2M_MASK (value: 2).
    /// </summary>
    Le2M = 2,

    /// <summary>
    ///     Coded PHY (Long Range, Bluetooth 5.0+).
    ///     Maps to ScanSettings.PHY_LE_CODED_MASK (value: 4).
    /// </summary>
    LeCoded = 4
}
