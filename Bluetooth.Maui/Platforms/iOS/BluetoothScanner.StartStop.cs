namespace Bluetooth.Maui;

public partial class BluetoothScanner
{
    /// <summary>
    /// Gets or sets the list of service UUIDs to filter peripherals during scanning.
    /// </summary>
    /// <remarks>
    /// If empty, all peripherals will be discovered. If specified, only peripherals advertising these services will be reported.
    /// </remarks>
    public IReadOnlyList<CBUUID> PeripheralScanningServiceUuids { get; private set; } = Array.Empty<CBUUID>();

    /// <summary>
    /// Gets or sets the peripheral scanning options.
    /// </summary>
    /// <remarks>
    /// These options control whether to allow duplicate advertisements and other scanning behaviors.
    /// </remarks>
    public PeripheralScanningOptions? PeripheralScanningOptions { get; private set; }

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this checks if the central manager's <see cref="CBCentralManager.IsScanning"/> property is <c>true</c>.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = CbCentralManagerProxy?.CbCentralManager.IsScanning ?? false;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Starts scanning for peripherals using the Core Bluetooth central manager.
    /// The method determines which overload to call based on available service UUIDs and options.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="CbCentralManagerProxy"/> is <c>null</c>.</exception>
    protected override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
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
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops scanning for peripherals using the Core Bluetooth central manager.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="CbCentralManagerProxy"/> is <c>null</c>.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbCentralManagerProxy);
        CbCentralManagerProxy.CbCentralManager.StopScan();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

}
