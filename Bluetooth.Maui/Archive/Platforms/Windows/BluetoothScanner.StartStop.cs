using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothScanner
{
    /// <summary>
    /// Gets the current status of the Bluetooth LE advertisement watcher.
    /// </summary>
    /// <remarks>
    /// Changing this property automatically updates <see cref="BaseBluetoothActivity.IsRunning"/>
    /// and triggers success events when transitioning to Started or Stopped states.
    /// </remarks>
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

    /// <summary>
    /// Waits asynchronously for the advertisement watcher to reach a specific status.
    /// </summary>
    /// <param name="state">The target watcher status to wait for.</param>
    /// <param name="timeout">The timeout for the wait operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that completes when the target status is reached.</returns>
    public ValueTask WaitForBluetoothLeAdvertisementWatcherStatusAsync(BluetoothLEAdvertisementWatcherStatus state, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(BluetoothLeAdvertisementWatcherStatus), state, timeout, cancellationToken);
    }

    /// <summary>
    /// Called when the advertisement watcher is stopped.
    /// </summary>
    /// <param name="argsError">The error code, if any.</param>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when the watcher stops with an error.</exception>
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

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, this checks if the advertisement watcher status is <see cref="BluetoothLEAdvertisementWatcherStatus.Started"/>.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        BluetoothLeAdvertisementWatcherStatus = BluetoothLeAdvertisementWatcherProxy?.BluetoothLeAdvertisementWatcher.Status ?? BluetoothLEAdvertisementWatcherStatus.Stopped;
        IsRunning = BluetoothLeAdvertisementWatcherStatus == BluetoothLEAdvertisementWatcherStatus.Started;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Starts the Windows Bluetooth LE advertisement watcher.
    /// </remarks>
    protected override ValueTask NativeStartAsync(IBluetoothScannerStartScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        BluetoothLeAdvertisementWatcherProxy = new BluetoothLeAdvertisementWatcherWrapper(this);
        BluetoothLeAdvertisementWatcherProxy.BluetoothLeAdvertisementWatcher.Start();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops the Windows Bluetooth LE advertisement watcher.
    /// </remarks>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        BluetoothLeAdvertisementWatcherProxy?.BluetoothLeAdvertisementWatcher.Stop();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }
}
