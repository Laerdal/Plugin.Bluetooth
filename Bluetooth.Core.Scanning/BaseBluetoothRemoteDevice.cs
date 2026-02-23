namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothRemoteDevice" />
public abstract partial class BaseBluetoothRemoteDevice : BaseBindableObject, IBluetoothRemoteDevice
{
    /// <summary>
    ///     The logger instance used for logging device operations.
    /// </summary>
    private readonly ILogger<IBluetoothRemoteDevice> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothRemoteDevice" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="request">The factory request containing device information.</param>
    /// <param name="serviceFactory">The factory for creating Bluetooth services.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected BaseBluetoothRemoteDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request, IBluetoothServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter, ILogger<IBluetoothRemoteDevice>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(scanner);
        ArgumentNullException.ThrowIfNull(serviceFactory);
        ArgumentNullException.ThrowIfNull(rssiToSignalStrengthConverter);
        ArgumentNullException.ThrowIfNull(request);

        _logger = logger ?? NullLogger<IBluetoothRemoteDevice>.Instance;
        Scanner = scanner;
        ServiceFactory = serviceFactory;
        RssiToSignalStrengthConverter = rssiToSignalStrengthConverter;
        Id = request.DeviceId;
        Manufacturer = request.Manufacturer;
        if (request.Advertisement != null)
        {
            OnAdvertisementReceived(request.Advertisement);
        }
    }

    /// <summary>
    ///     The factory responsible for creating services associated with this device.
    /// </summary>
    protected IBluetoothServiceFactory ServiceFactory { get; }

    /// <summary>
    ///     The converter responsible for translating RSSI values to signal strength levels, which can be used for filtering and sorting devices based on signal quality.
    /// </summary>
    protected IBluetoothRssiToSignalStrengthConverter RssiToSignalStrengthConverter { get; }

    /// <inheritdoc />
    public IBluetoothScanner Scanner { get; }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public Manufacturer Manufacturer { get; }

    /// <inheritdoc />
    public DateTimeOffset LastSeen { get; private set; }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <summary>
    ///     Performs the core disposal logic for the device, including disconnection, cleanup of pending operations, and resource disposal.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    protected async virtual ValueTask DisposeAsyncCore()
    {
        try
        {
            // Ensure device is disconnected
            await DisconnectIfNeededAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }

        // Clear RSSI history
        _rssiHistory.Clear();

        // Complete any pending tasks
        ConnectionTcs?.TrySetCanceled();
        DisconnectionTcs?.TrySetCanceled();
        ServicesExplorationTcs?.TrySetCanceled();
        RequestMtuTcs?.TrySetCanceled();
        SetPreferredPhyTcs?.TrySetCanceled();
        OpenL2CapChannelTcs?.TrySetCanceled();
        SignalStrengthReadingTcs?.TrySetCanceled();

        // Unsubscribe from events
        Services.CollectionChanged -= ServicesOnCollectionChanged;
        AdvertisementReceived = null;

        Connected = null;
        Disconnected = null;
        Connecting = null;
        Disconnecting = null;
        ConnectionStateChanged = null;
        UnexpectedDisconnection = null;

        ServiceListChanged = null;
        ServicesAdded = null;
        ServicesRemoved = null;

        MtuChanged = null;
        PhyChanged = null;

        await ClearServicesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Id}] {Name} by {Manufacturer}";
    }
}
