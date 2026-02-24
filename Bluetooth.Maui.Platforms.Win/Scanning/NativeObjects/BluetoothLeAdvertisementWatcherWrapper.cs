namespace Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

/// <summary>
///     Proxy class for Windows Bluetooth LE advertisement watcher that provides event handling
///     for advertisement scanning operations. Follows the iOS CbCentralManagerWrapper pattern.
/// </summary>
public sealed partial class BluetoothLeAdvertisementWatcherWrapper : BaseBindableObject, IBluetoothLeAdvertisementWatcherWrapper, IDisposable
{
    private BluetoothLEAdvertisementWatcher? _watcher;
    private readonly IBluetoothLeAdvertisementWatcherProxyDelegate _delegate;
    private readonly ITicker _ticker;
    private readonly Lock _lock = new Lock();
    private IDisposable? _refreshSubscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothLeAdvertisementWatcherWrapper" /> class.
    /// </summary>
    /// <param name="delegate">The delegate for handling advertisement watcher events.</param>
    /// <param name="ticker">The ticker for periodic property refresh.</param>
    public BluetoothLeAdvertisementWatcherWrapper(
        IBluetoothLeAdvertisementWatcherProxyDelegate @delegate,
        ITicker ticker)
    {
        ArgumentNullException.ThrowIfNull(@delegate);
        ArgumentNullException.ThrowIfNull(ticker);
        _delegate = @delegate;
        _ticker = ticker;
    }

    /// <summary>
    ///     Gets the native Windows Bluetooth LE advertisement watcher instance.
    ///     Lazily initializes the watcher on first access.
    /// </summary>
    public BluetoothLEAdvertisementWatcher BluetoothLeAdvertisementWatcher
    {
        get
        {
            if (_watcher == null)
            {
                lock (_lock)
                {
                    if (_watcher == null)
                    {
                        _watcher = new BluetoothLEAdvertisementWatcher();
                        _watcher.Received += BluetoothLEAdvertisementWatcher_Received;
                        _watcher.Stopped += BluetoothLEAdvertisementWatcher_Stopped;

                        // Start ticker for property refresh
                        _refreshSubscription = _ticker.Register(
                            "BluetoothLeAdvertisementWatcherWrapper",
                            TimeSpan.FromSeconds(1),
                            RefreshValues,
                            runImmediately: true);
                    }
                }
            }

            return _watcher;
        }
    }

    /// <summary>
    ///     Disposes the advertisement watcher and unsubscribes from events.
    /// </summary>
    public void Dispose()
    {
        if (_watcher != null)
        {
            _watcher.Received -= BluetoothLEAdvertisementWatcher_Received;
            _watcher.Stopped -= BluetoothLEAdvertisementWatcher_Stopped;
            _watcher.Stop();
        }

        _refreshSubscription?.Dispose();
    }

    /// <summary>
    ///     Handles advertisement watcher stopped events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The advertisement watcher that stopped.</param>
    /// <param name="args">The stopped event arguments.</param>
    private void BluetoothLEAdvertisementWatcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
    {
        try
        {
            _delegate.OnAdvertisementWatcherStopped(args.Error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Handles advertisement received events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The advertisement watcher that received the advertisement.</param>
    /// <param name="args">The advertisement received event arguments.</param>
    private void BluetoothLEAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        try
        {
            _delegate.OnAdvertisementReceived(args);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #region Observable Properties

    /// <summary>
    ///     Refreshes the properties of the Bluetooth LE advertisement watcher.
    ///     Called automatically by the ticker every second.
    /// </summary>
    private void RefreshValues()
    {
        if (_watcher == null)
        {
            return;
        }

        MaxOutOfRangeTimeout = _watcher.MaxOutOfRangeTimeout;
        MinOutOfRangeTimeout = _watcher.MinOutOfRangeTimeout;
        MaxSamplingInterval = _watcher.MaxSamplingInterval;
        MinSamplingInterval = _watcher.MinSamplingInterval;

        Status = _watcher.Status;
        ScanningMode = _watcher.ScanningMode;

        SignalStrengthFilterInRangeThresholdInDBm = _watcher.SignalStrengthFilter?.InRangeThresholdInDBm;
        SignalStrengthFilterOutOfRangeThresholdInDBm = _watcher.SignalStrengthFilter?.OutOfRangeThresholdInDBm;
        SignalStrengthFilterSamplingInterval = _watcher.SignalStrengthFilter?.SamplingInterval;
        SignalStrengthFilterOutOfRangeTimeout = _watcher.SignalStrengthFilter?.OutOfRangeTimeout;

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
        {
            AllowExtendedAdvertisements = _watcher.AllowExtendedAdvertisements;
        }
    }

    /// <inheritdoc/>
    public bool AllowExtendedAdvertisements
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public TimeSpan MaxOutOfRangeTimeout
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public TimeSpan MinOutOfRangeTimeout
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public TimeSpan MaxSamplingInterval
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public TimeSpan MinSamplingInterval
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public BluetoothLEAdvertisementWatcherStatus Status
    {
        get => GetValue(BluetoothLEAdvertisementWatcherStatus.Created);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public BluetoothLEScanningMode ScanningMode
    {
        get => GetValue(BluetoothLEScanningMode.Active);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public short? SignalStrengthFilterInRangeThresholdInDBm
    {
        get => GetValue<short?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public short? SignalStrengthFilterOutOfRangeThresholdInDBm
    {
        get => GetValue<short?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public TimeSpan? SignalStrengthFilterSamplingInterval
    {
        get => GetValue<TimeSpan?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public TimeSpan? SignalStrengthFilterOutOfRangeTimeout
    {
        get => GetValue<TimeSpan?>(null);
        private set => SetValue(value);
    }

    #endregion
}
