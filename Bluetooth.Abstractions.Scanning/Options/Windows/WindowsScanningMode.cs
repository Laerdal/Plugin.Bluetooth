namespace Bluetooth.Abstractions.Scanning.Options.Windows;

/// <summary>
///     Windows Bluetooth LE advertisement scanning mode.
/// </summary>
/// <remarks>
///     Maps to <c>Windows.Devices.Bluetooth.Advertisement.BluetoothLEScanningMode</c>.
/// </remarks>
public enum WindowsScanningMode
{
    /// <summary>
    ///     Passive scanning: no scan request is sent, so scan-response PDUs are never solicited.
    ///     Lowest power, but any manufacturer data that a device only advertises via scan response
    ///     will never be observed.
    /// </summary>
    Passive = 0,

    /// <summary>
    ///     Active scanning: a scan request is sent to scannable advertisers, soliciting a scan
    ///     response PDU. Required for <see cref="WindowsScanningOptions.MergeScanResponses" /> to
    ///     have anything to merge.
    /// </summary>
    Active = 1
}
