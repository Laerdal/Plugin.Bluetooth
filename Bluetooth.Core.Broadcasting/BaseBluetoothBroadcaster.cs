

namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth Low Energy broadcaster implementations that advertise the device's presence.
/// </summary>
/// <remarks>
/// Broadcasters allow a device to act as a BLE peripheral, advertising its presence and services to nearby devices.
/// This is the opposite role of a scanner, which listens for advertisements.
/// </remarks>
public abstract partial class BaseBluetoothBroadcaster : BaseBindableObject, IBluetoothBroadcaster
{
    #region Properties

    /// <summary>
    /// The logger instance used for logging broadcaster operations.
    /// </summary>
    private readonly ILogger<IBluetoothBroadcaster> _logger;

    /// <inheritdoc/>
    public IBluetoothAdapter Adapter { get; }

    /// <summary>
    /// Gets the factory for creating broadcast services.
    /// </summary>
    protected IBluetoothLocalServiceFactory LocalServiceFactory { get; }

    /// <summary>
    /// Gets the factory for creating broadcast client devices.
    /// </summary>
    protected IBluetoothConnectedDeviceFactory ConnectedDeviceFactory { get; }

    /// <inheritdoc/>
    public IBluetoothPermissionManager PermissionManager { get; }

    private readonly IDisposable? _refreshSubscription;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothBroadcaster"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter to associate with this broadcaster.</param>
    /// <param name="localServiceFactory">The factory for creating broadcast services.</param>
    /// <param name="connectedDeviceFactory">The factory for creating broadcast client devices.</param>
    /// <param name="permissionManager">The permission manager for handling Bluetooth permissions.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected BaseBluetoothBroadcaster(IBluetoothAdapter adapter,
        IBluetoothLocalServiceFactory localServiceFactory,
        IBluetoothConnectedDeviceFactory connectedDeviceFactory,
        IBluetoothPermissionManager permissionManager,
        ITicker ticker,
        ILogger<IBluetoothBroadcaster>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(localServiceFactory);
        ArgumentNullException.ThrowIfNull(connectedDeviceFactory);
        ArgumentNullException.ThrowIfNull(permissionManager);
        ArgumentNullException.ThrowIfNull(ticker);

        _logger = logger ?? NullLogger<IBluetoothBroadcaster>.Instance;
        Adapter = adapter;
        LocalServiceFactory = localServiceFactory;
        ConnectedDeviceFactory = connectedDeviceFactory;
        PermissionManager = permissionManager;
        _refreshSubscription = ticker.Register("Broadcaster Refresh Tick", TimeSpan.FromSeconds(2), RefreshAsync);
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
        return $"Broadcaster ({Services.Count}S/{ClientDevices.Count}C)";
    }

    #region Dispose

    /// <summary>
    /// Disposes the resources asynchronously.
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
