namespace Bluetooth.Maui.Platforms.Win.Broadcasting.NativeObjects;

/// <summary>
///     Proxy class for Windows Bluetooth LE advertisement publisher that provides event handling
///     for advertisement broadcasting operations. Follows the iOS CbCentralManagerWrapper pattern.
/// </summary>
public sealed partial class BluetoothLeAdvertisementPublisherWrapper : BaseBindableObject, IBluetoothLeAdvertisementPublisherWrapper, IDisposable
{
    private BluetoothLEAdvertisementPublisher? _publisher;

    private readonly IBluetoothLeAdvertisementPublisherProxyDelegate _delegate;

    private readonly ITicker _ticker;

    private readonly Lock _lock = new Lock();

    private IDisposable? _refreshSubscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothLeAdvertisementPublisherWrapper" /> class.
    /// </summary>
    /// <param name="delegate">The delegate for handling advertisement publisher events.</param>
    /// <param name="ticker">The ticker for periodic property refresh.</param>
    public BluetoothLeAdvertisementPublisherWrapper(IBluetoothLeAdvertisementPublisherProxyDelegate @delegate, ITicker ticker)
    {
        ArgumentNullException.ThrowIfNull(@delegate);
        ArgumentNullException.ThrowIfNull(ticker);
        _delegate = @delegate;
        _ticker = ticker;
    }

    /// <summary>
    ///     Gets the native Windows Bluetooth LE advertisement publisher instance.
    ///     Lazily initializes the publisher on first access.
    /// </summary>
    public BluetoothLEAdvertisementPublisher BluetoothLeAdvertisementPublisher
    {
        get
        {
            if (_publisher == null)
            {
                lock (_lock)
                {
                    _publisher = new BluetoothLEAdvertisementPublisher();
                    _publisher.StatusChanged += BluetoothLEAdvertisementPublisher_StatusChanged;

                    // Start ticker for property refresh
                    _refreshSubscription = _ticker.Register("BluetoothLeAdvertisementPublisherWrapper", TimeSpan.FromSeconds(1), RefreshValues, runImmediately: true);
                }
            }

            return _publisher;
        }
    }

    /// <summary>
    ///     Disposes the advertisement publisher and unsubscribes from events.
    /// </summary>
    public void Dispose()
    {
        if (_publisher != null)
        {
            _publisher.StatusChanged -= BluetoothLEAdvertisementPublisher_StatusChanged;
        }

        _refreshSubscription?.Dispose();
    }

    /// <summary>
    ///     Handles advertisement publisher status change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The advertisement publisher whose status changed.</param>
    /// <param name="args">The status change event arguments.</param>
    private void BluetoothLEAdvertisementPublisher_StatusChanged(BluetoothLEAdvertisementPublisher sender, BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
    {
        try
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621))
            {
                _delegate.OnAdvertisementPublisherStatusChanged(args.Status, args.Error, args.SelectedTransmitPowerLevelInDBm);
            }
            else
            {
                _delegate.OnAdvertisementPublisherStatusChanged(args.Status, args.Error);
            }
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #region Observable Properties

    /// <summary>
    ///     Refreshes the properties of the Bluetooth LE advertisement publisher.
    ///     Called automatically by the ticker every second.
    /// </summary>
    private void RefreshValues()
    {
        if (_publisher == null)
        {
            return;
        }

        Status = _publisher.Status;
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
        {
            IsAnonymous = _publisher.IsAnonymous;
            UseExtendedAdvertisement = _publisher.UseExtendedAdvertisement;
            IncludeTransmitPowerLevel = _publisher.IncludeTransmitPowerLevel;
            PreferredTransmitPowerLevelInDBm = _publisher.PreferredTransmitPowerLevelInDBm;
        }
    }

    /// <inheritdoc/>
    public BluetoothLEAdvertisementPublisherStatus Status
    {
        get => GetValue(BluetoothLEAdvertisementPublisherStatus.Created);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public bool IsAnonymous
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public bool UseExtendedAdvertisement
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public bool IncludeTransmitPowerLevel
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public short? PreferredTransmitPowerLevelInDBm
    {
        get => GetValue<short?>(null);
        private set => SetValue(value);
    }

    #endregion

}
