namespace Bluetooth.Abstractions.Scanning.Options.Windows;

/// <summary>
///     Windows platform-specific scanning options.
/// </summary>
/// <remarks>
///     These options map to <c>Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementWatcher</c>
///     members not covered (or not fully covered) by the cross-platform <see cref="ScanningOptions" />
///     properties. See ADR 0003 (<c>Docs/Architecture/ADR/0003-windows-scan-response-handling.md</c>)
///     for the background on scan-response handling specifically.
/// </remarks>
public record WindowsScanningOptions
{
    /// <summary>
    ///     Gets whether ADV and SCAN_RSP PDUs for the same advertising interval are buffered and
    ///     merged into a single <see cref="Bluetooth.Abstractions.Scanning.IBluetoothAdvertisement" />,
    ///     matching how Android/iOS already deliver advertisements.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Defaults to <c>false</c>: advertisements dispatch immediately with no buffering, same
    ///         as today. Scan responses are instead dispatched separately via
    ///         <c>WindowsBluetoothRemoteDevice.ScanResponseReceived</c> / <c>.LastScanResponse</c>,
    ///         once the device already exists in the scanner's device list — a device is never
    ///         created from a scan response alone.
    ///     </para>
    ///     <para>
    ///         When <c>true</c>: each ADV PDU is held for up to <see cref="ScanResponseMergeWindow" />
    ///         waiting for a matching SCAN_RSP PDU before dispatching, merging their manufacturer
    ///         data. On timeout the advertisement fires as-is (ADV-only). <c>ScanResponseReceived</c>
    ///         / <c>LastScanResponse</c> still fire afterward whenever a scan response was actually
    ///         received, in both modes.
    ///     </para>
    ///     <para>
    ///         Requires active scanning to ever receive anything to merge — scan responses are never
    ///         sent to a passive scanner. See <see cref="ScanningMode" />.
    ///     </para>
    /// </remarks>
    public bool MergeScanResponses { get; init; }

    /// <summary>
    ///     Gets how long to wait for a scan response before dispatching an advertisement as-is, when
    ///     <see cref="MergeScanResponses" /> is <c>true</c>. Ignored otherwise.
    /// </summary>
    /// <remarks>
    ///     Defaults to 500ms — a starting point, not yet validated against real hardware. Tune once
    ///     tested (see ADR 0003 follow-up).
    /// </remarks>
    public TimeSpan ScanResponseMergeWindow { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    ///     Gets the Bluetooth LE scanning mode (active vs. passive). When <c>null</c> (default), the
    ///     watcher keeps scanning actively, matching this plugin's existing behavior.
    /// </summary>
    /// <remarks>
    ///     Takes precedence over the cross-platform <see cref="ScanningOptions.ScanMode" /> mapping
    ///     when set. Passive scanning never solicits scan responses — incompatible with
    ///     <see cref="MergeScanResponses" /> and with ever observing
    ///     <c>WindowsBluetoothRemoteDevice.ScanResponseReceived</c>.
    /// </remarks>
    public WindowsScanningMode? ScanningMode { get; init; }

    /// <summary>
    ///     Gets whether to enable reception of advertisements using the Extended Advertising format
    ///     (Bluetooth 5.0+, requires Windows 10 version 2004 / build 19041 or later). When
    ///     <c>null</c> (default), falls back to the cross-platform
    ///     <see cref="ScanningOptions.EnableExtendedAdvertising" /> value.
    /// </summary>
    public bool? AllowExtendedAdvertisements { get; init; }

    /// <summary>
    ///     Gets the out-of-range RSSI threshold in dBm used by the native signal strength filter. Has
    ///     no cross-platform equivalent — <see cref="ScanningOptions.RssiThreshold" /> only ever maps
    ///     to the native in-range threshold.
    /// </summary>
    public short? SignalStrengthOutOfRangeThresholdInDBm { get; init; }

    /// <summary>
    ///     Gets the sampling interval used by the native signal strength filter to aggregate RSSI
    ///     events. Has no cross-platform equivalent.
    /// </summary>
    public TimeSpan? SignalStrengthSamplingInterval { get; init; }

    /// <summary>
    ///     Gets the timeout after which a device is considered out of range by the native signal
    ///     strength filter. Has no cross-platform equivalent.
    /// </summary>
    public TimeSpan? SignalStrengthOutOfRangeTimeout { get; init; }
}
