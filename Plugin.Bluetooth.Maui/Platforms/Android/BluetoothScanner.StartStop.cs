using Plugin.Bluetooth.Maui.PlatformSpecific;
using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;
using Plugin.Bluetooth.Maui.PlatformSpecific.NativeOptions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothScanner
{
    protected override void NativeRefreshIsRunning()
    {
        //IsRunning = BluetoothAdapterProxy.BluetoothAdapter.IsDiscovering;
        //On android there is no way to check if Scan is running ... IsDiscovering only refers to Classic Bluetooth ...
        // Tracking of this is done manually when starting/stopping the scan and when receiving advertisements
    }

    public StartScanningOptions? StartScanningOptions { get; set; }

    protected override void NativeStart()
    {
        ArgumentNullException.ThrowIfNull(BluetoothLeScannerProxy.BluetoothLeScanner, nameof(BluetoothLeScannerProxy.BluetoothLeScanner));
        BluetoothLeScannerProxy.BluetoothLeScanner.StartScan(StartScanningOptions.ToNativeScanFilters(), StartScanningOptions.ToNativeScanSettings(), ScanCallbackProxy);
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

    protected override void NativeStop()
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
    }

}
