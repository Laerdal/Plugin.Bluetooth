namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothScanner" />
public abstract partial class BaseBluetoothScanner : BaseBindableObject, IBluetoothScanner
{

    /// <summary>
    ///     The Bluetooth adapter associated with this scanner, used for performing scanning operations and managing Bluetooth interactions.
    /// </summary>
    public IBluetoothAdapter Adapter { get; }

    /// <summary>
    ///     The provider for Bluetooth device names, used to resolve human-readable names for devices based on their identifiers. This allows for a more user-friendly representation of devices in the scanner's device list and related events.
    /// </summary>
    public IBluetoothNameProvider? NameProvider { get; }

    /// <summary>
    ///     The factory for creating options related to Bluetooth scanning, such as signal strength smoothing and other scanning parameters. This allows for flexible configuration of scanning behavior and properties, enabling developers to customize the scanning experience based on their application's needs and the specific requirements of different platforms.
    /// </summary>
    public IOptionsFactory<ScanningOptions> ScanningOptionsFactory { get; }

    /// <summary>
    ///     The factory for creating options related to signal strength smoothing in Bluetooth scanning. This allows for configuring how the signal strength readings are averaged over time to provide a more stable and accurate representation of the device's signal strength, especially in environments with fluctuating signal conditions.
    /// </summary>
    public IOptionsFactory<SignalStrengthSmoothingOptions> SignalStrengthSmoothingOptionsFactory { get; }

    /// <summary>
    ///     The options for smoothing signal strength jitter in Bluetooth scanning. This allows for configuring how the signal strength readings are averaged over time to provide a more stable and accurate representation of the device's signal strength, especially in environments with fluctuating signal conditions.
    /// </summary>
    protected IOptionsMonitor<ScanningOptions> ScanningOptions { get; }

    /// <summary>
    ///     The converter used to convert RSSI values to signal strength levels. This allows for consistent interpretation of signal strength across different platforms and devices, as RSSI values can vary in scale and meaning depending on the underlying Bluetooth implementation.
    /// </summary>
    public IBluetoothRssiToSignalStrengthConverter RssiToSignalStrengthConverter { get; }

    /// <summary>
    ///     The logger factory used for creating loggers within the scanner and its related components. This allows for consistent logging across the scanner and its associated devices, services, and characteristics, and enables the creation of loggers with specific categories for different parts of the Bluetooth scanning implementation.
    /// </summary>
    public ILoggerFactory? LoggerFactory { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothScanner" /> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this scanner.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="nameProvider">Optional provider for Bluetooth device names.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    [ActivatorUtilitiesConstructor]
    protected BaseBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ITicker ticker,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null) : base(loggerFactory?.CreateLogger<IBluetoothScanner>())
    {
        // Validate constructor arguments
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(rssiToSignalStrengthConverter);
        ArgumentNullException.ThrowIfNull(ticker);

        Adapter = adapter;
        LoggerFactory = loggerFactory;
        RssiToSignalStrengthConverter = rssiToSignalStrengthConverter;
        NameProvider = nameProvider;

        _refreshSubscription = ticker.Register("Scanner Refresh Tick", TimeSpan.FromSeconds(2), RefreshAsync);
    }

    /// <summary>
    ///     Refreshes the broadcaster's properties and state.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <remarks>
    ///     This method is called periodically by the ticker to ensure the broadcaster's properties and state are up-to-date. Derived classes should override this method to implement the logic for refreshing the broadcaster's properties, such
    ///     as checking the broadcasting state, updating the list of active services, or any other relevant information. The base implementation throws a <see cref="NotImplementedException" />, indicating that derived classes must provide their
    ///     own implementation of the refresh logic specific to the platform or implementation.
    /// </remarks>
    protected virtual Task RefreshAsync(CancellationToken cancellationToken)
    {
        NativeRefreshIsRunning();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        lock (Devices)
        {
            return $"Scanner ({Devices.Count}D)";
        }
    }

    private readonly IDisposable? _refreshSubscription;

    /// <summary>
    ///     Platform-specific implementation to check if scanner permissions are granted.
    /// </summary>
    /// <returns>True if permissions are granted, otherwise false.</returns>
    /// <remarks>
    ///     Implement platform-specific permission checks. Should NOT throw exceptions.
    ///     Return false if permissions cannot be determined.
    /// </remarks>
    protected abstract ValueTask<bool> NativeHasScannerPermissionsAsync();

    /// <summary>
    ///     Platform-specific implementation to request scanner permissions.
    /// </summary>
    /// <param name="requireBackgroundLocation">Android-only: whether to request background location.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the permission request operation.</param>
    /// <exception cref="Exception">Throw platform-specific exceptions (SecurityException, COMException, etc.) on failure.</exception>
    /// <remarks>
    ///     Implement platform-specific permission request dialogs.
    ///     Throw native exceptions on failure - base class will wrap them in BluetoothPermissionException.
    /// </remarks>
    protected abstract ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken);

    /// <inheritdoc />
    public async ValueTask<bool> HasScannerPermissionsAsync()
    {
        try
        {
            return await NativeHasScannerPermissionsAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogScannerPermissionCheckFailed(ex);
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask RequestScannerPermissionsAsync(bool requireBackgroundLocation = false, CancellationToken cancellationToken = default)
    {
        try
        {
            await NativeRequestScannerPermissionsAsync(requireBackgroundLocation, cancellationToken).ConfigureAwait(false);
        }
        catch (BluetoothPermissionException)
        {
            throw; // Already wrapped, re-throw
        }
        catch (Exception ex)
        {
            throw new BluetoothPermissionException("Failed to request scanner permissions. Ensure required permissions are declared in your app manifest/Info.plist.", ex);
        }
    }

    #region Dispose

    /// <summary>
    ///     Disposes the resources asynchronously.
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
