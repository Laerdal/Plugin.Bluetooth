namespace Bluetooth.Maui.PlatformSpecific;


/// <summary>
/// Proxy class for Windows Bluetooth LE advertisement watcher that provides event handling
/// for advertisement scanning operations.
/// </summary>
public sealed partial class BluetoothLeAdvertisementWatcherProxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothLeAdvertisementWatcherProxy"/> class.
    /// </summary>
    /// <param name="bluetoothAdapterProxyDelegate">The delegate for handling advertisement watcher events.</param>
    public BluetoothLeAdvertisementWatcherProxy(IBluetoothLeAdvertisementWatcherProxyDelegate bluetoothAdapterProxyDelegate)
    {
        BluetoothLeAdvertisementWatcherProxyDelegate = bluetoothAdapterProxyDelegate;
        BluetoothLeAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();
        BluetoothLeAdvertisementWatcher.Received += BluetoothLEAdvertisementWatcher_Received;
        BluetoothLeAdvertisementWatcher.Stopped += BluetoothLEAdvertisementWatcher_Stopped;
    }

    /// <summary>
    /// Gets the delegate responsible for handling advertisement watcher events.
    /// </summary>
    private IBluetoothLeAdvertisementWatcherProxyDelegate BluetoothLeAdvertisementWatcherProxyDelegate { get; }

    /// <summary>
    /// Gets the native Windows Bluetooth LE advertisement watcher instance.
    /// </summary>
    public BluetoothLEAdvertisementWatcher BluetoothLeAdvertisementWatcher { get; }

    /// <summary>
    /// Handles advertisement watcher stopped events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The advertisement watcher that stopped.</param>
    /// <param name="args">The stopped event arguments.</param>
    private void BluetoothLEAdvertisementWatcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
    {
        try
        {
            BluetoothLeAdvertisementWatcherProxyDelegate.OnAdvertisementWatcherStopped(args.Error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    /// Handles advertisement received events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The advertisement watcher that received the advertisement.</param>
    /// <param name="args">The advertisement received event arguments.</param>
    private void BluetoothLEAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        try
        {
            BluetoothLeAdvertisementWatcherProxyDelegate.OnAdvertisementReceived(args);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}

