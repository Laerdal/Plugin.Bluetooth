namespace Plugin.Bluetooth.BaseClasses;

/// <inheritdoc cref="IBluetoothDevice" />
public abstract partial class BaseBluetoothDevice : BaseBindableObject, IBluetoothDevice
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothDevice"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="id">The unique identifier of the device.</param>
    /// <param name="manufacturer">The manufacturer of the device.</param>
    protected BaseBluetoothDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer)
    {
        Scanner = scanner;
        Id = id;
        Manufacturer = manufacturer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothDevice"/> class from an advertisement.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="advertisement">The advertisement information used to initialize the device.</param>
    protected BaseBluetoothDevice(IBluetoothScanner scanner, IBluetoothAdvertisement advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);

        Scanner = scanner;
        Id = advertisement.BluetoothAddress;
        Manufacturer = advertisement.Manufacturer;
    }

    /// <inheritdoc/>
    public IBluetoothScanner Scanner { get; set; }

    /// <inheritdoc/>
    public string Id { get; set; }

    /// <inheritdoc/>
    public Manufacturer Manufacturer { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset LastSeen { get; private set; }

    /// <summary>
    /// Performs the core disposal logic for the device, including disconnection, cleanup of pending operations, and resource disposal.
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

        // Unsubscribe from events
        Services.CollectionChanged -= ServicesOnCollectionChanged;
        AdvertisementReceived = null;

        Connected = null;
        Disconnected = null;
        Connecting = null;
        Disconnecting = null;
        UnexpectedDisconnection = null;

        ServiceListChanged = null;
        ServicesAdded = null;
        ServicesRemoved = null;

        await ClearServicesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
