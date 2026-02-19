namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothScanner" />
public abstract partial class BaseBluetoothScanner : BaseBindableObject, IBluetoothScanner
{
    #region Properties

    /// <summary>
    /// The logger instance used for logging scanner operations.
    /// </summary>
    private readonly ILogger<IBluetoothScanner> _logger;

    /// <summary>
    /// The Bluetooth adapter associated with this scanner, used for performing scanning operations and managing Bluetooth interactions.
    /// </summary>
    public IBluetoothAdapter Adapter { get; }

    /// <summary>
    /// The factory responsible for creating Bluetooth devices managed by this scanner.
    /// </summary>
    protected IBluetoothDeviceFactory DeviceFactory { get; }

    /// <summary>
    /// The manager responsible for handling Bluetooth permissions required for scanning and device interactions.
    /// </summary>
    protected IBluetoothPermissionManager PermissionManager { get; }

    /// <summary>
    /// The converter responsible for translating RSSI values to signal strength levels, which can be used for filtering and sorting devices based on signal quality.
    /// </summary>
    protected IBluetoothRssiToSignalStrengthConverter RssiToSignalStrengthConverter { get; }

    private readonly IDisposable? _refreshSubscription;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this scanner.</param>
    /// <param name="permissionManager">The permission manager for handling Bluetooth permissions.</param>
    /// <param name="deviceFactory">The factory for creating Bluetooth devices.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected BaseBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothPermissionManager permissionManager,
        IBluetoothDeviceFactory deviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ITicker ticker,
        ILogger<IBluetoothScanner>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(permissionManager);
        ArgumentNullException.ThrowIfNull(deviceFactory);
        ArgumentNullException.ThrowIfNull(rssiToSignalStrengthConverter);
        ArgumentNullException.ThrowIfNull(ticker);

        _logger = logger ?? NullLogger<IBluetoothScanner>.Instance;
        Adapter = adapter;
        PermissionManager = permissionManager;
        DeviceFactory = deviceFactory;
        RssiToSignalStrengthConverter = rssiToSignalStrengthConverter;
        _refreshSubscription = ticker.Register("Scanner Refresh Tick", TimeSpan.FromSeconds(2), RefreshAsync);
    }

    #endregion

    #region Refresh

    /// <summary>
    /// Refreshes the broadcaster's properties and state.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <remarks>
    /// This method is called periodically by the ticker to ensure the broadcaster's properties and state are up-to-date. Derived classes should override this method to implement the logic for refreshing the broadcaster's properties, such
    /// as checking the broadcasting state, updating the list of active services, or any other relevant information. The base implementation throws a <see cref="NotImplementedException"/>, indicating that derived classes must provide their
    /// own implementation of the refresh logic specific to the platform or implementation.
    /// </remarks>
    protected virtual Task RefreshAsync(CancellationToken cancellationToken)
    {
        NativeRefreshIsRunning();
        return Task.CompletedTask;
    }

    #endregion

    /// <inheritdoc />
    public override string ToString()
    {
        lock (Devices)
        {
            return $"Scanner ({Devices.Count}D)";
        }
    }

    #region Dispose

    /// <summary>
    /// Disposes the resources asynchronously.
    /// </summary>
    protected async virtual ValueTask DisposeAsyncCore()
    {
        try
        {
            // Ensure scanner is stopped
            await StopScanningIfNeededAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }

        // Complete any pending tasks
        StartTcs?.TrySetCanceled();
        StopTcs?.TrySetCanceled();

        // Clear and dispose all devices
        await ClearDevicesAsync().ConfigureAwait(false);

        // Unsubscribe from events
        Devices.CollectionChanged -= DevicesOnCollectionChanged;

        // Dispose of the refresh subscription
        _refreshSubscription?.Dispose();

        // Clear all event handlers
        AdvertisementReceived = null;

        RunningStateChanged = null;
        Starting = null;
        Started = null;
        Stopping = null;
        Stopped = null;

        DeviceListChanged = null;
        DevicesAdded = null;
        DevicesRemoved = null;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    #endregion

}
