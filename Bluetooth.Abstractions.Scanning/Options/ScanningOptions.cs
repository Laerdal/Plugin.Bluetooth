using Bluetooth.Abstractions.Options;

namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents Bluetooth scanner configuration options.
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

    #endregion

    #region Advertisement Filtering

    /// <summary>
    ///     Advertisement filter of this scan. If set, only advertisements that pass the filter will be processed.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When <c>null</c> (default), the scan adds no filter. When set, only advertisements where the filter returns <c>true</c> are
    ///         processed, for as long as the scan runs.
    ///     </para>
    ///     <para>
    ///         It is combined with <see cref="IBluetoothScanner.AdvertisementFilter" />, which stays on the scanner between scans: an
    ///         advertisement has to pass both. On Windows without <c>WindowsScanningOptions.MergeScanResponses</c> an advertisement and its
    ///         scan response are separate advertisements, and the filter judges each of them on its own.
    ///     </para>
    /// </remarks>
    public Func<IBluetoothAdvertisement, bool>? AdvertisementFilter { get; init; }

    /// <summary>
    ///     Gets a value indicating whether duplicate advertisements should be ignored.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <b>Not applied yet:</b> no platform reads this value, so <see langword="true" /> and <see langword="false" /> scan the same
    ///         way. What each platform does with repeated advertisements today:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <b>Android</b>: with the default <see cref="CallbackType" />, <see cref="BluetoothScanCallbackType.AllMatches" />, every
    ///             advertisement is reported. <see cref="BluetoothScanCallbackType.FirstMatch" /> reports only the first advertisement of each
    ///             device; the callback types with <c>MatchLost</c> report that a device went away, not its advertisements.
    ///         </item>
    ///         <item>
    ///             <b>iOS/macOS</b>: unless <c>AppleScanningOptions.AllowDuplicates</c> is <see langword="true" />, CoreBluetooth coalesces repeated
    ///             advertisements of a peripheral into one discovery event. That is not a guarantee of one callback per device (changed
    ///             advertisement data can produce another), so key your handler on the device when you need strict deduplication. A plain
    ///             <see cref="ScanningOptions" /> passes no scan options, which is the same as not allowing duplicates.
    ///         </item>
    ///         <item><b>Windows</b>: every advertisement is reported.</item>
    ///     </list>
    /// </remarks>
    public bool IgnoreDuplicateAdvertisements { get; init; }

    /// <summary>
    ///     Gets a value indicating whether advertisements without a local name should be ignored.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When <see langword="true" />, an advertisement whose <see cref="IBluetoothAdvertisement.DeviceName" /> is empty is dropped
    ///         before it raises <see cref="IBluetoothScanner.AdvertisementReceived" /> or reaches a device, so a device that never
    ///         advertises its name is never discovered. Only the name the advertisement carries counts, not one the platform has cached.
    ///     </para>
    ///     <para>
    ///         Windows delivers an advertisement and its scan response as separate advertisements unless
    ///         <c>WindowsScanningOptions.MergeScanResponses</c> is on, and a name is often only in the scan response: without merging, the
    ///         advertisement is dropped and only the scan response gets through. Turn merging on when you use this option on Windows.
    ///     </para>
    /// </remarks>
    public bool IgnoreNamelessAdvertisements { get; init; }

    #endregion

    #region Service UUID Filtering

    /// <summary>
    ///     Gets the list of service UUIDs to filter during scanning.
    ///     When set, only devices advertising at least one of these services will be discovered.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item>
    ///             <b>Android</b>: Applied natively, one ScanFilter per UUID. With <c>AndroidScanningOptions.ManufacturerIds</c> set as well,
    ///             a device has to match one UUID and one company identifier.
    ///         </item>
    ///         <item>
    ///             <b>iOS/macOS</b>: Applied natively, as the serviceUUIDs argument of CBCentralManager scanForPeripherals.
    ///             <c>AppleScanningOptions.ServiceUuid</c> is added to this list; it cannot be set together with
    ///             <c>AppleScanningOptions.PeripheralUuids</c> (an <c>ArgumentException</c>). When <c>PeripheralUuids</c> is set on its
    ///             own, this list is ignored. iOS requires service UUIDs to scan while the app is in the background.
    ///         </item>
    ///         <item>
    ///             <b>Windows</b>: Applied in software, after the advertisement is received: the radio still receives everything, so
    ///             it saves no power. An advertisement passes when it lists one of the UUIDs or when a PDU of the same address did in
    ///             the last 10 seconds, because the UUID can be in the advertisement or in its scan response, which Windows delivers
    ///             as separate PDUs. With <c>WindowsScanningOptions.MergeScanResponses</c> the filter sees both.
    ///         </item>
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
    /// <remarks>
    ///     Applies to the devices this scan discovers. A device keeps the smoothing it was created with, so another scan with other
    ///     options does not change the devices that are already in the list. A window below 1 is treated as 1, which is no smoothing.
    /// </remarks>
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

    /// <summary>
    ///     Gets the inactivity timeout after which a device is considered disappeared.
    /// </summary>
    /// <remarks>
    ///     When null (default), no inactivity-based disappearance handling is applied.
    ///     When set, devices with no new advertisement for at least this duration are treated
    ///     according to <see cref="DeviceDisappearanceBehavior" />.
    /// </remarks>
    public TimeSpan? DeviceDisappearTimeout { get; init; }

    /// <summary>
    ///     Gets how devices should be handled once they exceed <see cref="DeviceDisappearTimeout" />.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see cref="BluetoothDeviceDisappearanceBehavior.MarkAsStale" /> to avoid breaking callers
    ///     that expect previously discovered devices to remain accessible.
    /// </remarks>
    public BluetoothDeviceDisappearanceBehavior DeviceDisappearanceBehavior { get; init; } = BluetoothDeviceDisappearanceBehavior.MarkAsStale;

    #endregion

    #region Platform-Specific Scanning Options

    /// <summary>
    ///     Gets the Android platform-specific scanning options.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Should be an instance of <c>Bluetooth.Abstractions.Scanning.Options.Android.AndroidScanningOptions</c>.
    ///         These options are only used on Android platforms and are ignored on other platforms.
    ///     </para>
    ///     <para>
    ///         Provides access to Android-specific scan settings such as:
    ///     </para>
    ///     <list type="bullet">
    ///         <item><b>MatchMode</b>: Control aggressive vs sticky matching (API 23+)</item>
    ///         <item><b>NumOfMatches</b>: Set advertisement match count before reporting (API 23+)</item>
    ///         <item><b>ReportDelay</b>: Batch scan results for power savings (API 23+)</item>
    ///         <item><b>ManufacturerIds</b>: Native filter on Bluetooth SIG company identifiers, so other devices cost no callback</item>
    ///         <item><b>Phy</b>: Select Bluetooth 5.0 PHY layers (API 26+)</item>
    ///         <item><b>Legacy</b>: Filter for legacy-only advertisements (API 26+)</item>
    ///     </list>
    /// </remarks>
    public object? Android { get; init; }

    /// <summary>
    ///     Gets the Windows platform-specific scanning options.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Should be an instance of <c>Bluetooth.Abstractions.Scanning.Options.Windows.WindowsScanningOptions</c>.
    ///         These options are only used on Windows platforms and are ignored on other platforms.
    ///     </para>
    ///     <para>
    ///         Provides access to Windows-specific scan settings such as:
    ///     </para>
    ///     <list type="bullet">
    ///         <item><b>MergeScanResponses</b>: merge ADV/SCAN_RSP PDUs into one advertisement, matching Android/iOS</item>
    ///         <item><b>ScanningMode</b>: active vs. passive scanning</item>
    ///         <item><b>AllowExtendedAdvertisements</b>: Bluetooth 5.0+ extended advertising support</item>
    ///         <item>Native signal strength filter knobs with no cross-platform equivalent</item>
    ///     </list>
    /// </remarks>
    public object? Windows { get; init; }

    #endregion
}
