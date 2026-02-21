namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth Low Energy broadcaster implementations that advertise the device's presence.
/// </summary>
/// <remarks>
///     Broadcasters allow a device to act as a BLE peripheral, advertising its presence and services to nearby devices.
///     This is the opposite role of a scanner, which listens for advertisements.
/// </remarks>
public abstract partial class BaseBluetoothBroadcaster : BaseBindableObject, IBluetoothBroadcaster
{
    #region Constructor

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothBroadcaster" /> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter to associate with this broadcaster.</param>
    /// <param name="localServiceFactory">The factory for creating broadcast services.</param>
    /// <param name="connectedDeviceFactory">The factory for creating broadcast client devices.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected BaseBluetoothBroadcaster(IBluetoothAdapter adapter,
        IBluetoothLocalServiceFactory localServiceFactory,
        IBluetoothConnectedDeviceFactory connectedDeviceFactory,
        ITicker ticker,
        ILogger<IBluetoothBroadcaster>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(localServiceFactory);
        ArgumentNullException.ThrowIfNull(connectedDeviceFactory);
        ArgumentNullException.ThrowIfNull(ticker);

        _logger = logger ?? NullLogger<IBluetoothBroadcaster>.Instance;
        Adapter = adapter;
        LocalServiceFactory = localServiceFactory;
        ConnectedDeviceFactory = connectedDeviceFactory;
        _refreshSubscription = ticker.Register("Broadcaster Refresh Tick", TimeSpan.FromSeconds(2), RefreshAsync);
    }

    #endregion

    #region Refresh

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

    #endregion

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Broadcaster ({Services.Count}S/{ClientDevices.Count}C)";
    }

    #region Properties

    /// <summary>
    ///     The logger instance used for logging broadcaster operations.
    /// </summary>
    private readonly ILogger<IBluetoothBroadcaster> _logger;

    /// <inheritdoc />
    public IBluetoothAdapter Adapter { get; }

    /// <summary>
    ///     Gets the factory for creating broadcast services.
    /// </summary>
    protected IBluetoothLocalServiceFactory LocalServiceFactory { get; }

    /// <summary>
    ///     Gets the factory for creating broadcast client devices.
    /// </summary>
    protected IBluetoothConnectedDeviceFactory ConnectedDeviceFactory { get; }

    private readonly IDisposable? _refreshSubscription;

    #endregion

    #region Abstract Permission Methods

    /// <summary>
    ///     Platform-specific implementation to check if broadcaster permissions are granted.
    /// </summary>
    /// <returns>True if permissions are granted, otherwise false.</returns>
    /// <remarks>
    ///     Implement platform-specific permission checks. Should NOT throw exceptions.
    ///     Return false if permissions cannot be determined.
    /// </remarks>
    protected abstract ValueTask<bool> NativeHasBroadcasterPermissionsAsync();

    /// <summary>
    ///     Platform-specific implementation to request broadcaster permissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the permission request operation.</param>
    /// <exception cref="Exception">Throw platform-specific exceptions on failure.</exception>
    /// <remarks>
    ///     Implement platform-specific permission request dialogs.
    ///     Throw native exceptions on failure - base class will wrap them in BluetoothPermissionException.
    /// </remarks>
    protected abstract ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken);

    #endregion

    #region IBluetoothBroadcaster Permission Methods

    /// <inheritdoc />
    public async ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        try
        {
            return await NativeHasBroadcasterPermissionsAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogBroadcasterPermissionCheckFailed(ex);
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask RequestBroadcasterPermissionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await NativeRequestBroadcasterPermissionsAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (BluetoothPermissionException)
        {
            throw; // Already wrapped, re-throw
        }
        catch (Exception ex)
        {
            throw new BluetoothPermissionException(
                "Failed to request broadcaster permissions. Ensure required permissions are declared in your app manifest/Info.plist.",
                ex);
        }
    }

    #endregion

    #region Dispose

    /// <summary>
    ///     Disposes the resources asynchronously.
    /// </summary>
    protected async virtual ValueTask DisposeAsyncCore()
    {
        await StopBroadcastingAsync().ConfigureAwait(false);
        await RemoveAllServicesAsync().ConfigureAwait(false);
        _refreshSubscription?.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    #endregion
}