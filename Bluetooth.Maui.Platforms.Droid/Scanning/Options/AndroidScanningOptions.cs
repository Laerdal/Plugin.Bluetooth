namespace Bluetooth.Maui.Platforms.Droid.Scanning.Options;

#pragma warning disable CA1008 // Enums should have zero value

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
    ///             <see cref="ScanMatchNumber.One"/>: Report after matching one advertisement (fastest discovery, default)
    ///         </item>
    ///         <item>
    ///             <see cref="ScanMatchNumber.Few"/>: Report after matching a few advertisements
    ///         </item>
    ///         <item>
    ///             <see cref="ScanMatchNumber.Max"/>: Report after matching many advertisements (reduces callbacks)
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Requires Android 6.0 (API 23) or higher. Ignored on lower API levels.
    ///     </para>
    /// </remarks>
    public ScanMatchNumber? ScanMatchNumber { get; init; }

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

#pragma warning restore CA1008 // Enums should have zero value
