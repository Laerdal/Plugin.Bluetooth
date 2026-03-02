namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothRemoteDevice" />
public abstract partial class BaseBluetoothRemoteDevice : BaseBindableObject, IBluetoothRemoteDevice
{
    /// <inheritdoc />
    public IBluetoothScanner Scanner { get; }
    
    /// <summary>
    ///    Initializes a new instance of the <see cref="BaseBluetoothRemoteDevice" /> class using the provided Bluetooth advertisement and parent scanner.
    /// </summary>
    /// <param name="parentScanner">The Bluetooth scanner associated with this remote device.</param>
    /// <param name="advertisement">The Bluetooth advertisement containing information about the remote device.</param>
    /// <param name="logger">Optional logger instance for logging purposes.</param>
    protected BaseBluetoothRemoteDevice(IBluetoothScanner parentScanner, IBluetoothAdvertisement advertisement, ILogger<IBluetoothRemoteDevice>? logger = null) : base(logger)
    {
        // Validate constructor arguments
        ArgumentNullException.ThrowIfNull(advertisement);
        ArgumentNullException.ThrowIfNull(parentScanner);

        Scanner = parentScanner;
        Id = advertisement.BluetoothAddress;
        Manufacturer = advertisement.Manufacturer;

        // Properties from spec
        OnAdvertisementReceived(advertisement);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothRemoteDevice" /> class.
    /// </summary>
    /// <param name="parentScanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="id">The unique identifier of the Bluetooth device, typically a UUID or MAC address.</param>
    /// <param name="manufacturer">The manufacturer information for the Bluetooth device.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected BaseBluetoothRemoteDevice(
        IBluetoothScanner parentScanner,
        string id,
        Manufacturer manufacturer,
        ILogger<IBluetoothRemoteDevice>? logger = null) : base(logger)
    {
        // Validate constructor arguments
        ArgumentNullException.ThrowIfNull(parentScanner);

        Scanner = parentScanner;
        Id = id;
        Manufacturer = manufacturer;
    }


    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public Manufacturer Manufacturer { get; }

    /// <inheritdoc />
    public DateTimeOffset LastSeen { get; private set; }

    #region Dispose

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

    #endregion

    #region ToString

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Id}] {Name} by {Manufacturer}";
    }

    #endregion
}
