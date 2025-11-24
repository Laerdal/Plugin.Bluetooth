namespace Plugin.Bluetooth.Maui;

public partial class BluetoothScanner
{
    public IReadOnlyList<CBUUID> PeripheralScanningServiceUuids { get; private set; } = Array.Empty<CBUUID>();

    public PeripheralScanningOptions? PeripheralScanningOptions { get; private set; }

    protected override void NativeRefreshIsRunning()
    {
        IsRunning = CbCentralManagerProxy?.CbCentralManager.IsScanning ?? false;
    }

    protected override void NativeStart()
    {
        ArgumentNullException.ThrowIfNull(CbCentralManagerProxy);

        // Determine which ScanForPeripherals overload to call based on available parameters
        var hasServiceUuids = PeripheralScanningServiceUuids.Count > 0;
        var hasOptions = PeripheralScanningOptions is not null;

        if (hasServiceUuids && hasOptions)
        {
            // Call with both service UUIDs and options
            CbCentralManagerProxy.CbCentralManager.ScanForPeripherals(
                PeripheralScanningServiceUuids.ToArray(),
                PeripheralScanningOptions);
        }
        else if (hasServiceUuids && !hasOptions)
        {
            // Call with only service UUIDs (options will default to null)
            CbCentralManagerProxy.CbCentralManager.ScanForPeripherals(
                PeripheralScanningServiceUuids.ToArray());
        }
        else if (!hasServiceUuids && hasOptions)
        {
            // Call with null service UUIDs but with options
            CbCentralManagerProxy.CbCentralManager.ScanForPeripherals(
                null,
                PeripheralScanningOptions);
        }
        else
        {
            // Call with null for both (scan for all peripherals with no options)
            CbCentralManagerProxy.CbCentralManager.ScanForPeripherals(
                (CBUUID[]?)null);
        }

        NativeRefreshIsRunning();
    }

    protected override void NativeStop()
    {
        ArgumentNullException.ThrowIfNull(CbCentralManagerProxy);
        CbCentralManagerProxy.CbCentralManager.StopScan();
        NativeRefreshIsRunning();
    }

}
