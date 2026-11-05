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
    ///         Defaults to <c>false</c>: every PDU, advertisements and scan responses alike, dispatches
    ///         immediately with no buffering, same as before this option existed. A scan response
    ///         additionally raises <c>WindowsBluetoothRemoteDevice.ScanResponseReceived</c> and sets
    ///         <c>.LastScanResponse</c> on its device.
    ///     </para>
    ///     <para>
    ///         When <c>true</c>: each ADV PDU is held for up to <see cref="ScanResponseMergeWindow" />
    ///         waiting for a matching SCAN_RSP PDU before dispatching, merging their manufacturer
    ///         data and service data. A device is never created from a scan response alone. If no
    ///         scan response arrives in time, the advertisement is merged with the last scan response
    ///         received from that address when it is younger than <see cref="ScanResponseMaxAge" />,
    ///         and fires as-is (ADV-only) otherwise. <c>ScanResponseReceived</c> / <c>LastScanResponse</c>
    ///         still fire whenever a scan response was actually received, in both modes.
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
    ///     Defaults to 500ms. Raise it for a peripheral that answers a scan request slowly. Must be between
    ///     zero and 10 seconds when <see cref="MergeScanResponses" /> is <c>true</c>; scanning does not start
    ///     with any other value. An advertisement is never held longer than this, whatever arrives from
    ///     the same address in the meantime.
    /// </remarks>
    public TimeSpan ScanResponseMergeWindow { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    ///     Gets how long the last scan response received from an address stays usable to complete a later
    ///     advertisement from the same address whose own scan response did not arrive in time, when
    ///     <see cref="MergeScanResponses" /> is <c>true</c>. Ignored otherwise.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A peripheral does not always answer a scan request, so without this an advertisement whose
    ///         scan response was missed would lose a name or service data that the previous one carried.
    ///         Defaults to 10 seconds. <see cref="TimeSpan.Zero" /> turns it off: a missed scan response then
    ///         leaves an advertisement-only event.
    ///     </para>
    ///     <para>
    ///         The remembered data can be that old when it is used, so a value that changes quickly
    ///         (a battery level) is as stale as this allows.
    ///     </para>
    /// </remarks>
    public TimeSpan ScanResponseMaxAge { get; init; } = TimeSpan.FromSeconds(10);

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
