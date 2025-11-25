using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothScanner
{

    public BluetoothLEAdvertisementWatcherStatus BluetoothLeAdvertisementWatcherStatus
    {
        get => GetValue(BluetoothLEAdvertisementWatcherStatus.Stopped);
        private set
        {
            if (SetValue(value))
            {
                NativeRefreshIsRunning();
                if (value == BluetoothLEAdvertisementWatcherStatus.Started)
                {
                    OnStartSucceeded();
                }
                else if (value == BluetoothLEAdvertisementWatcherStatus.Stopped)
                {
                    OnStopSucceeded();
                }
            }
        }
    }

    public ValueTask WaitForBluetoothLeAdvertisementWatcherStatusAsync(BluetoothLEAdvertisementWatcherStatus state, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(BluetoothLeAdvertisementWatcherStatus), state, timeout, cancellationToken);
    }

    public void OnAdvertisementWatcherStopped(BluetoothError argsError)
    {
        try
        {
            if (argsError != BluetoothError.Success)
            {
                throw new WindowsNativeBluetoothException(argsError);
            }

            OnStopSucceeded();
        }
        catch (Exception e)
        {
            OnStopFailed(e);
        }
    }

    protected override void NativeRefreshIsRunning()
    {
        BluetoothLeAdvertisementWatcherStatus = BluetoothLeAdvertisementWatcherProxy?.BluetoothLeAdvertisementWatcher.Status ?? BluetoothLEAdvertisementWatcherStatus.Stopped;
        IsRunning = BluetoothLeAdvertisementWatcherStatus == BluetoothLEAdvertisementWatcherStatus.Started;
    }

    protected override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        BluetoothLeAdvertisementWatcherProxy = new BluetoothLeAdvertisementWatcherProxy(this);
        BluetoothLeAdvertisementWatcherProxy.BluetoothLeAdvertisementWatcher.Start();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        BluetoothLeAdvertisementWatcherProxy?.BluetoothLeAdvertisementWatcher.Stop();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }
}
