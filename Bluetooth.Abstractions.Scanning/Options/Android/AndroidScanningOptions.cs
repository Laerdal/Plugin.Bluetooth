namespace Bluetooth.Abstractions.Scanning.Options.Android;

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

    /// <summary>
    ///     Gets the Bluetooth SIG company identifiers to filter on natively: only devices whose advertisement carries manufacturer-specific
    ///     data for one of them are reported.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Bluetooth stack applies it, as one <c>android.bluetooth.le.ScanFilter</c> per identifier, so the advertisements of every
    ///         other device never reach the app and cost it no callback. Only the identifier is compared, not the data that follows it. The
    ///         identifiers are the 16-bit numbers of the Bluetooth SIG company list, for example
    ///         <c>(int) Manufacturer.Laerdal_Medical_AS</c>; a value outside 0 to 65535 makes the scan fail to start with an
    ///         <see cref="ArgumentOutOfRangeException" />. <see langword="null" /> or an empty list filters nothing.
    ///     </para>
    ///     <para>
    ///         When <see cref="ScanningOptions.ServiceUuids" /> is set as well, a device has to match both lists: one of the UUIDs and one of
    ///         the identifiers.
    ///     </para>
    ///     <para>
    ///         This is a pre-filter for Android only, other platforms ignore it. Put the same condition in an advertisement filter when the
    ///         code has to behave the same everywhere. The stack judges each received packet, so how it treats a scan response that carries no
    ///         manufacturer data of its own can differ between device makers: check devices that send data in their scan response on the
    ///         hardware you target.
    ///     </para>
    /// </remarks>
    public IReadOnlyList<int>? ManufacturerIds { get; init; }
}

#pragma warning restore CA1008 // Enums should have zero value
