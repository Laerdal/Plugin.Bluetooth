namespace Bluetooth.Maui;

/// <summary>
///     Unified Bluetooth remote device facade providing cross-platform inheritance and extension points.
/// </summary>
/// <remarks>
///     <para>
///         This class wraps platform-specific device implementations and provides a unified
///         API with virtual extension points for client customization.
///     </para>
///     <para>
///         <b>Client Extensibility:</b> Inherit from this class to create typed devices:
///     </para>
///     <example>
///     <code>
///     public class SimManDevice : BluetoothRemoteDevice
///     {
///         public SimManDevice(IBluetoothRemoteDevice platformDevice) : base(platformDevice) { }
///
///         public Guid[] ExpectedServices { get; } = new[] { HeartRate, Respiratory };
///
///         protected override void OnConnected()
///         {
///             Logger.Info("SimMan connected!");
///             base.OnConnected();
///         }
///     }
///     </code>
///     </example>
/// </remarks>
public class BluetoothRemoteDevice : IBluetoothRemoteDevice
{
    #region Platform-Specific Device

    /// <summary>
    ///     The underlying platform-specific device implementation.
    /// </summary>
    private readonly IBluetoothRemoteDevice _platformDevice;

    /// <summary>
    ///     Gets the underlying platform-specific device implementation.
    /// </summary>
    /// <remarks>
    ///     Clients can access platform-specific APIs by casting this property using conditional compilation.
    /// </remarks>
    public IBluetoothRemoteDevice PlatformDevice => _platformDevice;

    #endregion

    #region Constructor

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothRemoteDevice"/> class.
    /// </summary>
    /// <param name="platformDevice">The platform-specific device implementation to wrap.</param>
    public BluetoothRemoteDevice(IBluetoothRemoteDevice platformDevice)
    {
        ArgumentNullException.ThrowIfNull(platformDevice);
        _platformDevice = platformDevice;

        // Hook up connection events to extension points
        _platformDevice.Connecting += (s, e) =>
        {
            OnConnecting();
            Connecting?.Invoke(this, e);
        };

        _platformDevice.Connected += (s, e) =>
        {
            OnConnected();
            Connected?.Invoke(this, e);
        };

        _platformDevice.Disconnecting += (s, e) =>
        {
            OnDisconnecting();
            Disconnecting?.Invoke(this, e);
        };

        _platformDevice.Disconnected += (s, e) =>
        {
            OnDisconnected();
            Disconnected?.Invoke(this, e);
        };

        // Forward other events
        _platformDevice.ConnectionStateChanged += (s, e) => ConnectionStateChanged?.Invoke(this, e);
        _platformDevice.UnexpectedDisconnection += (s, e) => UnexpectedDisconnection?.Invoke(this, e);
        _platformDevice.AdvertisementReceived += (s, e) => AdvertisementReceived?.Invoke(this, e);
        _platformDevice.ServiceListChanged += (s, e) => ServiceListChanged?.Invoke(this, e);
        _platformDevice.ServicesAdded += (s, e) => ServicesAdded?.Invoke(this, e);
        _platformDevice.ServicesRemoved += (s, e) => ServicesRemoved?.Invoke(this, e);
        _platformDevice.MtuChanged += (s, e) => MtuChanged?.Invoke(this, e);
        _platformDevice.PhyChanged += (s, e) => PhyChanged?.Invoke(this, e);
        _platformDevice.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, e);
    }

    #endregion

    #region Extension Points for Client Projects

    /// <summary>
    ///     Virtual method called when the device is connecting.
    ///     Override this in client projects to add custom connection initialization logic.
    /// </summary>
    /// <remarks>
    ///     This method is called before the device is fully connected.
    ///     Use this to prepare for connection, log events, or initialize state.
    /// </remarks>
    protected virtual void OnConnecting()
    {
        // Default: no-op, clients can override
    }

    /// <summary>
    ///     Virtual method called when the device has connected.
    ///     Override this in client projects to add custom post-connection logic.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method is called after the device is fully connected.
    ///         Use this to start service discovery, subscribe to characteristics, or initialize device state.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Automatically explore services</item>
    ///         <item>Subscribe to notification characteristics</item>
    ///         <item>Read device information</item>
    ///         <item>Update UI state</item>
    ///     </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override void OnConnected()
    /// {
    ///     Task.Run(async () =>
    ///     {
    ///         await ExploreServicesAsync(ServiceExplorationOptions.Full);
    ///         var heartRateService = GetServiceOrDefault(HeartRateServiceId);
    ///         if (heartRateService != null)
    ///         {
    ///             var characteristic = await heartRateService.GetCharacteristicAsync(HeartRateCharId);
    ///             await characteristic.SubscribeToNotificationsAsync();
    ///         }
    ///     });
    ///     base.OnConnected();
    /// }
    /// </code>
    /// </example>
    protected virtual void OnConnected()
    {
        // Default: no-op, clients can override
    }

    /// <summary>
    ///     Virtual method called when the device is disconnecting.
    ///     Override this in client projects to add custom disconnection preparation logic.
    /// </summary>
    /// <remarks>
    ///     This method is called before the device is fully disconnected.
    ///     Use this to clean up resources, save state, or log events.
    /// </remarks>
    protected virtual void OnDisconnecting()
    {
        // Default: no-op, clients can override
    }

    /// <summary>
    ///     Virtual method called when the device has disconnected.
    ///     Override this in client projects to add custom post-disconnection logic.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method is called after the device is fully disconnected.
    ///         Use this to clean up resources, update UI, or handle reconnection logic.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Clear cached data</item>
    ///         <item>Update UI state</item>
    ///         <item>Trigger reconnection attempts</item>
    ///         <item>Log disconnection events</item>
    ///     </list>
    /// </remarks>
    protected virtual void OnDisconnected()
    {
        // Default: no-op, clients can override
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - Core Properties

    /// <inheritdoc />
    public IBluetoothScanner Scanner => _platformDevice.Scanner;

    /// <inheritdoc />
    public string Id => _platformDevice.Id;

    /// <inheritdoc />
    public Manufacturer Manufacturer => _platformDevice.Manufacturer;

    /// <inheritdoc />
    public string AdvertisedName => _platformDevice.AdvertisedName;

    /// <inheritdoc />
    public string Name => _platformDevice.Name;

    /// <inheritdoc />
    public string CachedName => _platformDevice.CachedName;

    /// <inheritdoc />
    public string DebugName => _platformDevice.DebugName;

    /// <inheritdoc />
    public DateTimeOffset LastSeen => _platformDevice.LastSeen;

    #endregion

    #region IBluetoothRemoteDevice Implementation - Connection State

    /// <inheritdoc />
    public bool IsConnected => _platformDevice.IsConnected;

    /// <inheritdoc />
    public bool IsConnecting => _platformDevice.IsConnecting;

    /// <inheritdoc />
    public bool IsDisconnecting => _platformDevice.IsDisconnecting;

    /// <inheritdoc />
    public bool IgnoreNextUnexpectedDisconnection
    {
        get => _platformDevice.IgnoreNextUnexpectedDisconnection;
        set => _platformDevice.IgnoreNextUnexpectedDisconnection = value;
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - Events

    /// <inheritdoc />
    public event EventHandler? Connecting;

    /// <inheritdoc />
    public event EventHandler? Connected;

    /// <inheritdoc />
    public event EventHandler? Disconnecting;

    /// <inheritdoc />
    public event EventHandler? Disconnected;

    /// <inheritdoc />
    public event EventHandler<DeviceConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <inheritdoc />
    public event EventHandler<DeviceUnexpectedDisconnectionEventArgs>? UnexpectedDisconnection;

    /// <inheritdoc />
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Scanning.EventArgs.ServiceListChangedEventArgs>? ServiceListChanged;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Scanning.EventArgs.ServicesAddedEventArgs>? ServicesAdded;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Scanning.EventArgs.ServicesRemovedEventArgs>? ServicesRemoved;

    /// <inheritdoc />
    public event EventHandler<Abstractions.Scanning.EventArgs.MtuChangedEventArgs>? MtuChanged;

    /// <inheritdoc />
    public event EventHandler<PhyChangedEventArgs>? PhyChanged;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region IBluetoothRemoteDevice Implementation - Connection Methods

    /// <inheritdoc />
    public virtual ValueTask ConnectAsync(
        ConnectionOptions? connectionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.ConnectAsync(connectionOptions, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public virtual ValueTask ConnectIfNeededAsync(
        ConnectionOptions? connectionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.ConnectIfNeededAsync(connectionOptions, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public virtual ValueTask DisconnectAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.DisconnectAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public virtual ValueTask DisconnectIfNeededAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.DisconnectIfNeededAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask WaitForIsConnectedAsync(
        bool isConnected,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.WaitForIsConnectedAsync(isConnected, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask RequestConnectionPriorityAsync(
        ConnectionPriority priority,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.RequestConnectionPriorityAsync(priority, timeout, cancellationToken);
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - Service Discovery

    /// <inheritdoc />
    public bool IsExploringServices => _platformDevice.IsExploringServices;

    /// <inheritdoc />
    public virtual Task ExploreServicesAsync(
        ServiceExplorationOptions? options = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.ExploreServicesAsync(options, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask ClearServicesAsync()
    {
        return _platformDevice.ClearServicesAsync();
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - Service Access

    /// <inheritdoc />
    public IBluetoothRemoteService GetService(Func<IBluetoothRemoteService, bool> filter)
    {
        return _platformDevice.GetService(filter);
    }

    /// <inheritdoc />
    public IBluetoothRemoteService GetService(Guid id)
    {
        return _platformDevice.GetService(id);
    }

    /// <inheritdoc />
    public IBluetoothRemoteService? GetServiceOrDefault(Func<IBluetoothRemoteService, bool> filter)
    {
        return _platformDevice.GetServiceOrDefault(filter);
    }

    /// <inheritdoc />
    public IBluetoothRemoteService? GetServiceOrDefault(Guid id)
    {
        return _platformDevice.GetServiceOrDefault(id);
    }

    /// <inheritdoc />
    public IReadOnlyList<IBluetoothRemoteService> GetServices(Func<IBluetoothRemoteService, bool>? filter = null)
    {
        return _platformDevice.GetServices(filter);
    }

    /// <inheritdoc />
    public bool HasService(Func<IBluetoothRemoteService, bool> filter)
    {
        return _platformDevice.HasService(filter);
    }

    /// <inheritdoc />
    public bool HasService(Guid id)
    {
        return _platformDevice.HasService(id);
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - Signal Strength

    /// <inheritdoc />
    public int SignalStrengthDbm => _platformDevice.SignalStrengthDbm;

    /// <inheritdoc />
    public double SignalStrengthPercent => _platformDevice.SignalStrengthPercent;

    /// <inheritdoc />
    public ValueTask<int> ReadSignalStrengthAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.ReadSignalStrengthAsync(timeout, cancellationToken);
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - MTU

    /// <inheritdoc />
    public int Mtu => _platformDevice.Mtu;

    /// <inheritdoc />
    public ValueTask<int> RequestMtuAsync(
        int requestedMtu,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.RequestMtuAsync(requestedMtu, timeout, cancellationToken);
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - Advertisement

    /// <inheritdoc />
    public IBluetoothAdvertisement? LastAdvertisement => _platformDevice.LastAdvertisement;

    /// <inheritdoc />
    public TimeSpan IntervalBetweenAdvertisement => _platformDevice.IntervalBetweenAdvertisement;

    /// <inheritdoc />
    public void OnAdvertisementReceived(IBluetoothAdvertisement advertisement)
    {
        _platformDevice.OnAdvertisementReceived(advertisement);
    }

    /// <inheritdoc />
    public ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(
        Func<IBluetoothAdvertisement?, bool> filter,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.WaitForAdvertisementAsync(filter, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.WaitForAdvertisementAsync(timeout, cancellationToken);
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - Identity

    /// <inheritdoc />
    public ValueTask WaitForNameToChangeAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.WaitForNameToChangeAsync(timeout, cancellationToken);
    }

    #endregion



    #region IBluetoothRemoteDevice Implementation - PHY

    /// <inheritdoc />
    public PhyMode CurrentTxPhy => _platformDevice.CurrentTxPhy;

    /// <inheritdoc />
    public PhyMode CurrentRxPhy => _platformDevice.CurrentRxPhy;

    /// <inheritdoc />
    public ValueTask SetPreferredPhyAsync(
        PhyMode txPhy,
        PhyMode rxPhy,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.SetPreferredPhyAsync(txPhy, rxPhy, timeout, cancellationToken);
    }

    #endregion

    #region IBluetoothRemoteDevice Implementation - L2CAP

    /// <inheritdoc />
    public ValueTask<IBluetoothRemoteL2CapChannel> OpenL2CapChannelAsync(
        int psm,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDevice.OpenL2CapChannelAsync(psm, timeout, cancellationToken);
    }

    #endregion

    #region IAsyncDisposable

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_platformDevice is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync().ConfigureAwait(false);
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}
