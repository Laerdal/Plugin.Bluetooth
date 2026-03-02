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

    /// <inheritdoc />
    public IBluetoothAdapter Adapter { get; }

    /// <inheritdoc />
    public ILoggerFactory? LoggerFactory { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothBroadcaster" /> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter to associate with this broadcaster.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    protected BaseBluetoothBroadcaster(IBluetoothAdapter adapter, ITicker ticker, ILoggerFactory? loggerFactory = null) :
        base(loggerFactory?.CreateLogger<BaseBluetoothBroadcaster>())
    {
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(ticker);

        Adapter = adapter;
        LoggerFactory = loggerFactory;
        _refreshSubscription = ticker.Register("Broadcaster Refresh Tick", TimeSpan.FromSeconds(2), RefreshAsync);
    }

    #region Refresh

    private readonly IDisposable? _refreshSubscription;

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

    #region Permissions

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
            throw new BluetoothPermissionException("Failed to request broadcaster permissions. Ensure required permissions are declared in your app manifest/Info.plist.", ex);
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

    #region ToString

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Broadcaster ({Services.Count}S/{ClientDevices.Count}C)";
    }

    #endregion

}
