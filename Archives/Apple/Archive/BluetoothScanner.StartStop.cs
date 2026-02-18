using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothScanner
{
    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this checks if the central manager's <see cref="CBCentralManager.IsScanning"/> property is <c>true</c>.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = Adapter is BluetoothAdapter { CbCentralManagerIsScanning: true };
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Starts scanning for peripherals using the Core Bluetooth central manager.
    /// The method determines which overload to call based on available service UUIDs and options.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="BluetoothAdapter.CbCentralManagerWrapper"/> is <c>null</c>.</exception>
    protected override ValueTask NativeStartAsync(IBluetoothScannerStartScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        CbCentralManager ??= ((BluetoothAdapter)Adapter).GetCbCentralManagerWrapper(this, _options);
        CbCentralManager.ScanForPeripherals(scanningOptions);
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops scanning for peripherals using the Core Bluetooth central manager.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="BluetoothAdapter.CbCentralManagerWrapper"/> is <c>null</c>.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if(Adapter is not BluetoothAdapter bluetoothAdapter)
        {
            throw new InvalidOperationException("Adapter is not a BluetoothAdapter");
        }
        ArgumentNullException.ThrowIfNull(bluetoothAdapter.CbCentralManagerWrapper);
        bluetoothAdapter.CbCentralManagerWrapper.CbCentralManager.StopScan();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

}
