namespace Bluetooth.Maui;

/// <summary>
///     Unified Bluetooth broadcaster facade providing cross-platform inheritance and extension points.
/// </summary>
/// <remarks>
///     <para>
///         This class wraps platform-specific broadcaster implementations and provides a unified
///         API with virtual extension points for client customization.
///     </para>
///     <para>
///         <b>Client Extensibility:</b> Inherit from this class to add custom broadcasting logic:
///     </para>
///     <example>
///     <code>
///     public class MyCustomBroadcaster : BluetoothBroadcaster
///     {
///         public MyCustomBroadcaster(...) : base(...) { }
///
///         protected override ValueTask OnBroadcastStartingAsync(BroadcastingOptions? options, CancellationToken cancellationToken)
///         {
///             // Custom pre-broadcast setup
///             return base.OnBroadcastStartingAsync(options, cancellationToken);
///         }
///     }
///     </code>
///     </example>
/// </remarks>
public class BluetoothBroadcaster : IBluetoothBroadcaster
{
    #region Platform-Specific Broadcaster

#if __ANDROID__
    private readonly Platforms.Droid.Broadcasting.AndroidBluetoothBroadcaster _platformBroadcaster;
#elif __IOS__ || __MACCATALYST__
    private readonly Platforms.Apple.Broadcasting.AppleBluetoothBroadcaster _platformBroadcaster;
#elif WINDOWS
    private readonly Platforms.Win.Broadcasting.WindowsBluetoothBroadcaster _platformBroadcaster;
#else
    private readonly Platforms.DotNetCore.Broadcasting.DotNetCoreBluetoothBroadcaster _platformBroadcaster;
#endif

    /// <summary>
    ///     Gets the underlying platform-specific broadcaster implementation.
    /// </summary>
    /// <remarks>
    ///     Clients can access platform-specific APIs by casting this property using conditional compilation.
    /// </remarks>
    public IBluetoothBroadcaster PlatformBroadcaster => _platformBroadcaster;

    #endregion

    #region Constructor

#if __IOS__ || __MACCATALYST__
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothBroadcaster"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this broadcaster.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="cbPeripheralManagerOptions">Apple-specific: CBPeripheralManager initialization options (iOS/macOS only).</param>
    /// <param name="dispatchQueueProvider">Apple-specific: Dispatch queue provider for Core Bluetooth (iOS/macOS only).</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
#else
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothBroadcaster"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this broadcaster.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
#endif
    [ActivatorUtilitiesConstructor]
    public BluetoothBroadcaster(
        IBluetoothAdapter adapter,
        ITicker ticker,
#if __IOS__ || __MACCATALYST__
        IOptions<Platforms.Apple.Broadcasting.NativeObjects.CbPeripheralManagerOptions> cbPeripheralManagerOptions,
        IDispatchQueueProvider dispatchQueueProvider,
#endif
        ILoggerFactory? loggerFactory = null)
    {
        // Create platform-specific broadcaster instance
#if __ANDROID__
        _platformBroadcaster = new Platforms.Droid.Broadcasting.AndroidBluetoothBroadcaster(
            adapter,
            ticker,
            loggerFactory);
#elif __IOS__ || __MACCATALYST__
        _platformBroadcaster = new Platforms.Apple.Broadcasting.AppleBluetoothBroadcaster(
            adapter,
            ticker,
            cbPeripheralManagerOptions,
            dispatchQueueProvider,
            loggerFactory);
#elif WINDOWS
        _platformBroadcaster = new Platforms.Win.Broadcasting.WindowsBluetoothBroadcaster(
            adapter,
            ticker,
            loggerFactory);
#else
        _platformBroadcaster = new Platforms.DotNetCore.Broadcasting.DotNetCoreBluetoothBroadcaster(
            adapter,
            ticker,
            loggerFactory);
#endif

        // Hook up events from platform broadcaster to this facade
        _platformBroadcaster.RunningStateChanged += (s, e) => RunningStateChanged?.Invoke(this, e);
        _platformBroadcaster.Starting += (s, e) => Starting?.Invoke(this, e);
        _platformBroadcaster.Started += (s, e) => Started?.Invoke(this, e);
        _platformBroadcaster.Stopping += (s, e) => Stopping?.Invoke(this, e);
        _platformBroadcaster.Stopped += (s, e) => Stopped?.Invoke(this, e);
        _platformBroadcaster.ServiceListChanged += (s, e) => ServiceListChanged?.Invoke(this, e);
        _platformBroadcaster.ServicesAdded += (s, e) => ServicesAdded?.Invoke(this, e);
        _platformBroadcaster.ServicesRemoved += (s, e) => ServicesRemoved?.Invoke(this, e);
        _platformBroadcaster.ClientDeviceListChanged += (s, e) =>
        {
            OnClientDeviceListChanged(e);
            ClientDeviceListChanged?.Invoke(this, e);
        };
        _platformBroadcaster.ClientDevicesAdded += (s, e) =>
        {
            OnClientDevicesConnected(e.Items.ToList());
            ClientDevicesAdded?.Invoke(this, e);
        };
        _platformBroadcaster.ClientDevicesRemoved += (s, e) =>
        {
            OnClientDevicesDisconnected(e.Items.ToList());
            ClientDevicesRemoved?.Invoke(this, e);
        };
    }

    #endregion

    #region Extension Points for Client Projects

    /// <summary>
    ///     Virtual method called before broadcasting starts.
    ///     Override this in client projects to add custom pre-broadcast logic.
    /// </summary>
    /// <param name="options">The broadcasting options being used.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     <para>
    ///         This method is called after permissions are checked but before the native
    ///         platform broadcaster starts. Use this to perform setup, validation, or logging.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Configure platform-specific broadcast settings</item>
    ///         <item>Initialize custom state or advertising data</item>
    ///         <item>Validate broadcasting prerequisites</item>
    ///         <item>Start performance monitoring</item>
    ///     </list>
    /// </remarks>
    protected virtual ValueTask OnBroadcastStartingAsync(
        BroadcastingOptions? options,
        CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Virtual method called after broadcasting stops.
    ///     Override this in client projects to add custom post-broadcast cleanup logic.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     <para>
    ///         This method is called after the native platform broadcaster has stopped.
    ///         Use this to perform cleanup, logging, or final processing.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Flush cached advertising data</item>
    ///         <item>Log broadcast statistics</item>
    ///         <item>Clean up resources</item>
    ///         <item>Stop performance monitoring</item>
    ///     </list>
    /// </remarks>
    protected virtual ValueTask OnBroadcastStoppedAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Virtual method called when client devices connect to the broadcaster.
    ///     Override this in client projects to add custom connection handling logic.
    /// </summary>
    /// <param name="devices">The connected client devices.</param>
    /// <remarks>
    ///     <para>
    ///         This method is called on the platform's native thread/dispatcher.
    ///         Use this to track connections, initialize per-device state, or trigger custom events.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Track connected client IDs</item>
    ///         <item>Initialize per-device caches or state</item>
    ///         <item>Log connection events with device details</item>
    ///         <item>Trigger custom notifications to application layers</item>
    ///     </list>
    /// </remarks>
    protected virtual void OnClientDevicesConnected(IReadOnlyList<IBluetoothConnectedDevice> devices)
    {
        // Default: no-op, clients can override
    }

    /// <summary>
    ///     Virtual method called when client devices disconnect from the broadcaster.
    ///     Override this in client projects to add custom disconnection handling logic.
    /// </summary>
    /// <param name="devices">The disconnected client devices.</param>
    /// <remarks>
    ///     <para>
    ///         This method is called on the platform's native thread/dispatcher.
    ///         Use this to clean up per-device state or log disconnection events.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Clean up per-device caches or resources</item>
    ///         <item>Log disconnection events with device details</item>
    ///         <item>Update UI or application state</item>
    ///         <item>Trigger custom reconnection logic if needed</item>
    ///     </list>
    /// </remarks>
    protected virtual void OnClientDevicesDisconnected(IReadOnlyList<IBluetoothConnectedDevice> devices)
    {
        // Default: no-op, clients can override
    }

    /// <summary>
    ///     Virtual method called when the client device list changes.
    ///     Override this in client projects to add custom list change handling logic.
    /// </summary>
    /// <param name="eventArgs">The event arguments containing list change details.</param>
    /// <remarks>
    ///     <para>
    ///         This method is called on the platform's native thread/dispatcher.
    ///         Use this to react to overall list changes beyond just additions/removals.
    ///     </para>
    /// </remarks>
    protected virtual void OnClientDeviceListChanged(Abstractions.Broadcasting.EventArgs.ClientDeviceListChangedEventArgs eventArgs)
    {
        // Default: no-op, clients can override
    }

    #endregion

    #region IBluetoothBroadcaster Implementation - Properties

    /// <inheritdoc />
    public IBluetoothAdapter Adapter => _platformBroadcaster.Adapter;

    /// <inheritdoc />
    public ILoggerFactory? LoggerFactory => _platformBroadcaster.LoggerFactory;

    /// <inheritdoc />
    public BroadcastingOptions CurrentBroadcastingOptions => _platformBroadcaster.CurrentBroadcastingOptions;

    /// <inheritdoc />
    public bool IsRunning => _platformBroadcaster.IsRunning;

    /// <inheritdoc />
    public bool IsStarting => _platformBroadcaster.IsStarting;

    /// <inheritdoc />
    public bool IsStopping => _platformBroadcaster.IsStopping;

    #endregion

    #region IBluetoothBroadcaster Implementation - Events

    /// <inheritdoc />
    public event EventHandler? RunningStateChanged;

    /// <inheritdoc />
    public event EventHandler? Starting;

    /// <inheritdoc />
    public event EventHandler? Started;

    /// <inheritdoc />
    public event EventHandler? Stopping;

    /// <inheritdoc />
    public event EventHandler? Stopped;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Broadcasting.EventArgs.ServiceListChangedEventArgs>? ServiceListChanged;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Broadcasting.EventArgs.ServicesAddedEventArgs>? ServicesAdded;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Broadcasting.EventArgs.ServicesRemovedEventArgs>? ServicesRemoved;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Broadcasting.EventArgs.ClientDeviceListChangedEventArgs>? ClientDeviceListChanged;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Broadcasting.EventArgs.ClientDevicesAddedEventArgs>? ClientDevicesAdded;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Broadcasting.EventArgs.ClientDevicesRemovedEventArgs>? ClientDevicesRemoved;

    #endregion

    #region IBluetoothBroadcaster Implementation - Permissions

    /// <inheritdoc />
    public ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        return _platformBroadcaster.HasBroadcasterPermissionsAsync();
    }

    /// <inheritdoc />
    public ValueTask RequestBroadcasterPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return _platformBroadcaster.RequestBroadcasterPermissionsAsync(cancellationToken);
    }

    #endregion

    #region IBluetoothBroadcaster Implementation - Broadcasting Control

    /// <inheritdoc />
    public async ValueTask StartBroadcastingAsync(
        BroadcastingOptions? broadcastingOptions = null,
        Abstractions.Broadcasting.Options.PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await OnBroadcastStartingAsync(broadcastingOptions, cancellationToken).ConfigureAwait(false);
        await _platformBroadcaster.StartBroadcastingAsync(
            broadcastingOptions,
            permissionOptions,
            timeout,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask StartBroadcastingIfNeededAsync(
        BroadcastingOptions? broadcastingOptions = null,
        Abstractions.Broadcasting.Options.PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await _platformBroadcaster.StartBroadcastingIfNeededAsync(
            broadcastingOptions,
            permissionOptions,
            timeout,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask StopBroadcastingAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await _platformBroadcaster.StopBroadcastingAsync(timeout, cancellationToken).ConfigureAwait(false);
        await OnBroadcastStoppedAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask StopBroadcastingIfNeededAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await _platformBroadcaster.StopBroadcastingIfNeededAsync(timeout, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region IBluetoothBroadcaster Implementation - Service Management - Create

    /// <inheritdoc />
    public ValueTask<IBluetoothLocalService> CreateServiceAsync(
        Guid id,
        string? name = null,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformBroadcaster.CreateServiceAsync(id, name, isPrimary, timeout, cancellationToken);
    }

    #endregion

    #region IBluetoothBroadcaster Implementation - Service Management - Get

    /// <inheritdoc />
    public IBluetoothLocalService GetService(Func<IBluetoothLocalService, bool> filter)
    {
        return _platformBroadcaster.GetService(filter);
    }

    /// <inheritdoc />
    public IBluetoothLocalService GetService(Guid id)
    {
        return _platformBroadcaster.GetService(id);
    }

    /// <inheritdoc />
    public IBluetoothLocalService? GetServiceOrDefault(Func<IBluetoothLocalService, bool> filter)
    {
        return _platformBroadcaster.GetServiceOrDefault(filter);
    }

    /// <inheritdoc />
    public IBluetoothLocalService? GetServiceOrDefault(Guid id)
    {
        return _platformBroadcaster.GetServiceOrDefault(id);
    }

    /// <inheritdoc />
    public IReadOnlyList<IBluetoothLocalService> GetServices(
        Func<IBluetoothLocalService, bool>? filter = null)
    {
        return _platformBroadcaster.GetServices(filter);
    }

    #endregion

    #region IBluetoothBroadcaster Implementation - Service Management - Remove

    /// <inheritdoc />
    public ValueTask RemoveServiceAsync(
        Guid id,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformBroadcaster.RemoveServiceAsync(id, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask RemoveServiceAsync(
        IBluetoothLocalService localService,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformBroadcaster.RemoveServiceAsync(localService, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask RemoveAllServicesAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformBroadcaster.RemoveAllServicesAsync(timeout, cancellationToken);
    }

    #endregion

    #region IBluetoothBroadcaster Implementation - Service Management - Has

    /// <inheritdoc />
    public bool HasService(Func<IBluetoothLocalService, bool> filter)
    {
        return _platformBroadcaster.HasService(filter);
    }

    /// <inheritdoc />
    public bool HasService(Guid id)
    {
        return _platformBroadcaster.HasService(id);
    }

    #endregion

    #region IBluetoothBroadcaster Implementation - Client Device Management - Get

    /// <inheritdoc />
    public IBluetoothConnectedDevice GetClientDevice(Func<IBluetoothConnectedDevice, bool> filter)
    {
        return _platformBroadcaster.GetClientDevice(filter);
    }

    /// <inheritdoc />
    public IBluetoothConnectedDevice GetClientDevice(string id)
    {
        return _platformBroadcaster.GetClientDevice(id);
    }

    /// <inheritdoc />
    public IBluetoothConnectedDevice? GetClientDeviceOrDefault(Func<IBluetoothConnectedDevice, bool> filter)
    {
        return _platformBroadcaster.GetClientDeviceOrDefault(filter);
    }

    /// <inheritdoc />
    public IBluetoothConnectedDevice? GetClientDeviceOrDefault(string id)
    {
        return _platformBroadcaster.GetClientDeviceOrDefault(id);
    }

    /// <inheritdoc />
    public IReadOnlyList<IBluetoothConnectedDevice> GetClientDevices(
        Func<IBluetoothConnectedDevice, bool>? filter = null)
    {
        return _platformBroadcaster.GetClientDevices(filter);
    }

    #endregion

    #region IBluetoothBroadcaster Implementation - Client Device Management - Has

    /// <inheritdoc />
    public bool HasClientDevice(Func<IBluetoothConnectedDevice, bool> filter)
    {
        return _platformBroadcaster.HasClientDevice(filter);
    }

    /// <inheritdoc />
    public bool HasClientDevice(string id)
    {
        return _platformBroadcaster.HasClientDevice(id);
    }

    #endregion

    #region IAsyncDisposable

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_platformBroadcaster is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync().ConfigureAwait(false);
        }
    }

    #endregion
}
