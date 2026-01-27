namespace Bluetooth.Maui.PlatformSpecific;


/// <summary>
/// Proxy class for Windows Bluetooth LE advertisement watcher that provides event handling
/// for advertisement scanning operations.
/// </summary>
public sealed partial class BluetoothLeAdvertisementWatcherWrapper : BaseBindableObject, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothLeAdvertisementWatcherWrapper"/> class.
    /// </summary>
    /// <param name="scanner">The delegate for handling advertisement watcher events.</param>
    public BluetoothLeAdvertisementWatcherWrapper(IBluetoothLeAdvertisementWatcherProxyDelegate scanner)
    {
        BluetoothLeAdvertisementWatcherProxyDelegate = scanner;
        BluetoothLeAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();
        BluetoothLeAdvertisementWatcher.Received += BluetoothLEAdvertisementWatcher_Received;
        BluetoothLeAdvertisementWatcher.Stopped += BluetoothLEAdvertisementWatcher_Stopped;
    }

    /// <summary>
    /// Disposes the advertisement watcher and unsubscribes from events.
    /// </summary>
    public void Dispose()
    {
        BluetoothLeAdvertisementWatcher.Received -= BluetoothLEAdvertisementWatcher_Received;
        BluetoothLeAdvertisementWatcher.Stopped -= BluetoothLEAdvertisementWatcher_Stopped;
        BluetoothLeAdvertisementWatcher.Stop();
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

    #region Values

    /// <summary>
    /// Refreshes the properties of the Bluetooth LE advertisement watcher.
    /// </summary>
    public void RefreshValues()
    {
        BluetoothLeAdvertisementWatcherMaxOutOfRangeTimeout = BluetoothLeAdvertisementWatcher.MaxOutOfRangeTimeout;
        BluetoothLeAdvertisementWatcherMinOutOfRangeTimeout = BluetoothLeAdvertisementWatcher.MinOutOfRangeTimeout;
        BluetoothLeAdvertisementWatcherMaxSamplingInterval = BluetoothLeAdvertisementWatcher.MaxSamplingInterval;
        BluetoothLeAdvertisementWatcherMinSamplingInterval = BluetoothLeAdvertisementWatcher.MinSamplingInterval;

        BluetoothLeAdvertisementWatcherStatus = BluetoothLeAdvertisementWatcher.Status;
        BluetoothLeAdvertisementWatcherScanningMode = BluetoothLeAdvertisementWatcher.ScanningMode;

        BluetoothLeAdvertisementWatcherSignalStrengthFilterInRangeThresholdInDBm = BluetoothLeAdvertisementWatcher.SignalStrengthFilter?.InRangeThresholdInDBm;
        BluetoothLeAdvertisementWatcherSignalStrengthFilterOutOfRangeThresholdInDBm = BluetoothLeAdvertisementWatcher.SignalStrengthFilter?.OutOfRangeThresholdInDBm;
        BluetoothLeAdvertisementWatcherSignalStrengthFilterSamplingInterval = BluetoothLeAdvertisementWatcher.SignalStrengthFilter?.SamplingInterval;
        BluetoothLeAdvertisementWatcherSignalStrengthFilterOutOfRangeTimeout = BluetoothLeAdvertisementWatcher.SignalStrengthFilter?.OutOfRangeTimeout;

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
        {
            BluetoothLeAdvertisementWatcherAllowExtendedAdvertisements = BluetoothLeAdvertisementWatcher.AllowExtendedAdvertisements;
        }

    }


    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE advertisement watcher allows extended advertisements.
    /// </summary>
    public bool BluetoothLeAdvertisementWatcherAllowExtendedAdvertisements
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the maximum timeout for considering a device out of range.
    /// </summary>
    public TimeSpan BluetoothLeAdvertisementWatcherMaxOutOfRangeTimeout
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the minimum timeout for considering a device out of range.
    /// </summary>
    public TimeSpan BluetoothLeAdvertisementWatcherMinOutOfRangeTimeout
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the maximum sampling interval for the Bluetooth LE advertisement watcher.
    /// </summary>
    public TimeSpan BluetoothLeAdvertisementWatcherMaxSamplingInterval
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the minimum sampling interval for the Bluetooth LE advertisement watcher.
    /// </summary>
    public TimeSpan BluetoothLeAdvertisementWatcherMinSamplingInterval
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the current status of the Bluetooth LE advertisement watcher.
    /// </summary>
    public BluetoothLEAdvertisementWatcherStatus BluetoothLeAdvertisementWatcherStatus
    {
        get => GetValue(BluetoothLEAdvertisementWatcherStatus.Created);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the scanning mode used by the Bluetooth LE advertisement watcher.
    /// </summary>
    public BluetoothLEScanningMode BluetoothLeAdvertisementWatcherScanningMode
    {
        get => GetValue(BluetoothLEScanningMode.Active);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the in-range threshold for the signal strength filter of the Bluetooth LE advertisement watcher.
    /// </summary>
    public short? BluetoothLeAdvertisementWatcherSignalStrengthFilterInRangeThresholdInDBm
    {
        get => GetValue<short?>(null);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the out-of-range threshold for the signal strength filter of the Bluetooth LE advertisement watcher.
    /// </summary>
    public short? BluetoothLeAdvertisementWatcherSignalStrengthFilterOutOfRangeThresholdInDBm
    {
        get => GetValue<short?>(null);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the sampling interval for the signal strength filter of the Bluetooth LE advertisement watcher.
    /// </summary>
    public TimeSpan? BluetoothLeAdvertisementWatcherSignalStrengthFilterSamplingInterval
    {
        get => GetValue<TimeSpan?>(null);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the out-of-range timeout for the signal strength filter of the Bluetooth LE advertisement watcher.
    /// </summary>
    public TimeSpan? BluetoothLeAdvertisementWatcherSignalStrengthFilterOutOfRangeTimeout
    {
        get => GetValue<TimeSpan?>(null);
        private set => SetValue(value);
    }

    #endregion
}

