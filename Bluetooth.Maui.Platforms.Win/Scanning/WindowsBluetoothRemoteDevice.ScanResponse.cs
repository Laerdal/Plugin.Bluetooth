namespace Bluetooth.Maui.Platforms.Win.Scanning;

public partial class WindowsBluetoothRemoteDevice
{
    /// <summary>
    ///     Gets the most recent scan-response (SCAN_RSP) advertisement received for this device, if
    ///     any.
    /// </summary>
    /// <remarks>
    ///     Populated once a SCAN_RSP PDU has been correlated to this already-known device by
    ///     <see cref="Bluetooth.Maui.Platforms.Win.Scanning.WindowsBluetoothScanner" /> — a device is
    ///     never created from a scan response alone. Fires regardless of whether
    ///     <c>WindowsScanningOptions.MergeScanResponses</c> is enabled; see ADR 0003
    ///     (<c>Docs/Architecture/ADR/0003-windows-scan-response-handling.md</c>).
    /// </remarks>
    public WindowsBluetoothAdvertisement? LastScanResponse
    {
        get => GetValue<WindowsBluetoothAdvertisement?>(null);
        private set => SetValue(value);
    }

    /// <summary>
    ///     Occurs when a scan-response (SCAN_RSP) PDU is received for this device.
    /// </summary>
    public event EventHandler<AdvertisementReceivedEventArgs>? ScanResponseReceived;

    /// <summary>
    ///     Records a received scan response and raises <see cref="ScanResponseReceived" />.
    /// </summary>
    /// <param name="scanResponse">The scan-response advertisement received.</param>
    internal void OnScanResponseReceived(WindowsBluetoothAdvertisement scanResponse)
    {
        LastScanResponse = scanResponse;
        ScanResponseReceived?.Invoke(this, new AdvertisementReceivedEventArgs(scanResponse));
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore().ConfigureAwait(false);
        ScanResponseReceived = null;
    }
}
