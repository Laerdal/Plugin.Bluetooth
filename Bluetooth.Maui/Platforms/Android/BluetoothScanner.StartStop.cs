using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;
using Bluetooth.Maui.PlatformSpecific.NativeOptions;

namespace Bluetooth.Maui;

public partial class BluetoothScanner
{
    protected override void NativeRefreshIsRunning()
    {
        //IsRunning = BluetoothAdapterProxy.BluetoothAdapter.IsDiscovering;
        //On android there is no way to check if Scan is running ... IsDiscovering only refers to Classic Bluetooth ...
        // Tracking of this is done manually when starting/stopping the scan and when receiving advertisements
    }

    public StartScanningOptions? StartScanningOptions { get; set; }

    protected override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothLeScannerProxy.BluetoothLeScanner, nameof(BluetoothLeScannerProxy.BluetoothLeScanner));
        BluetoothLeScannerProxy.BluetoothLeScanner.StartScan(StartScanningOptions.ToNativeScanFilters(), StartScanningOptions.ToNativeScanSettings(), ScanCallbackProxy);
        return ValueTask.CompletedTask;
    }

    public virtual void OnScanFailed(ScanFailure errorCode)
    {
        if (errorCode == ScanFailure.AlreadyStarted)
        {
            IsRunning = true;
            return;
        }

        try
        {
            throw new AndroidNativeScanFailureException(errorCode);
        }
        catch (Exception e)
        {
            IsRunning = false;
            OnStartFailed(e);
        }
    }

    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(BluetoothLeScannerProxy.BluetoothLeScanner, nameof(BluetoothLeScannerProxy.BluetoothLeScanner));
            BluetoothLeScannerProxy.BluetoothLeScanner.StopScan(ScanCallbackProxy);
            IsRunning = false;
            OnStopSucceeded();
        }
        catch (Exception e)
        {
            IsRunning = false;
            OnStopFailed(e);
        }
        return ValueTask.CompletedTask;
    }

}
