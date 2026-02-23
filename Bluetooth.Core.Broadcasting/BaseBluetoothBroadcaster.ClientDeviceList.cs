namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothBroadcaster
{
    private ObservableCollection<IBluetoothConnectedDevice> ClientDevices
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += OnClientDevicesCollectionChanged;
            }

            return field;
        }
    }

    #region ClientDevices - Events

    /// <summary>
    ///     Handles collection change notifications for the <see cref="ClientDevices" /> collection.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="ea">Event arguments containing the collection change details.</param>
    private void OnClientDevicesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new ClientDeviceListChangedEventArgs(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            ClientDevicesAdded?.Invoke(this, new ClientDevicesAddedEventArgs(listChangedEventArgs.AddedItems));
        }

        if (listChangedEventArgs.RemovedItems != null)
        {
            ClientDevicesRemoved?.Invoke(this, new ClientDevicesRemovedEventArgs(listChangedEventArgs.RemovedItems));
        }

        ClientDeviceListChanged?.Invoke(this, listChangedEventArgs);
    }

    /// <inheritdoc />
    public event EventHandler<ClientDeviceListChangedEventArgs>? ClientDeviceListChanged;

    /// <inheritdoc />
    public event EventHandler<ClientDevicesAddedEventArgs>? ClientDevicesAdded;

    /// <inheritdoc />
    public event EventHandler<ClientDevicesRemovedEventArgs>? ClientDevicesRemoved;

    #endregion

    #region ClientDevices - Add/Remove

    /// <summary>
    ///     Adds a newly connected client device to the collection.
    ///     Called by platform implementations when a client connects.
    /// </summary>
    /// <param name="device">The client device to add.</param>
    protected void AddClientDevice(IBluetoothConnectedDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        ClientDevices.Add(device);
    }

    /// <summary>
    ///     Removes a client device from the collection.
    ///     Called by platform implementations when a client disconnects.
    /// </summary>
    /// <param name="device">The client device to remove.</param>
    protected void RemoveClientDevice(IBluetoothConnectedDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        ClientDevices.Remove(device);
    }

    /// <summary>
    ///     Removes a client device from the collection by ID.
    ///     Called by platform implementations when a client disconnects.
    /// </summary>
    /// <param name="id">The ID of the client device to remove.</param>
    protected void RemoveClientDevice(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var device = GetClientDevice(id);
        RemoveClientDevice(device);
    }

    #endregion

    #region ClientDevices - Get

    private readonly static Func<IBluetoothConnectedDevice, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc />
    public IBluetoothConnectedDevice GetClientDevice(Func<IBluetoothConnectedDevice, bool> filter)
    {
        return GetClientDeviceOrDefault(filter) ?? throw new ClientDeviceNotFoundException(this);
    }

    /// <inheritdoc />
    public IBluetoothConnectedDevice GetClientDevice(string id)
    {
        return GetClientDeviceOrDefault(id) ?? throw new ClientDeviceNotFoundException(this, id);
    }

    /// <inheritdoc />
    public IBluetoothConnectedDevice? GetClientDeviceOrDefault(Func<IBluetoothConnectedDevice, bool> filter)
    {
        lock (ClientDevices)
        {
            try
            {
                return ClientDevices.SingleOrDefault(filter);
            }
            catch (InvalidOperationException e)
            {
                throw new MultipleClientDevicesFoundException(this, ClientDevices.Where(filter).ToList(), e);
            }
            // Let collection-modified exceptions propagate (indicates bug)
        }
    }

    /// <inheritdoc />
    public IBluetoothConnectedDevice? GetClientDeviceOrDefault(string id)
    {
        lock (ClientDevices)
        {
            try
            {
                return ClientDevices.SingleOrDefault(device => device.Id == id);
            }
            catch (InvalidOperationException e)
            {
                throw new MultipleClientDevicesFoundException(this, id, ClientDevices.Where(device => device.Id == id).ToList(), e);
            }
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IBluetoothConnectedDevice> GetClientDevices(Func<IBluetoothConnectedDevice, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;

        lock (ClientDevices)
        {
            return ClientDevices.Where(filter).ToList();
        }
    }

    #endregion

    #region ClientDevices - Has

    /// <inheritdoc />
    public bool HasClientDevice(Func<IBluetoothConnectedDevice, bool> filter)
    {
        lock (ClientDevices)
        {
            return ClientDevices.Any(filter);
        }
    }

    /// <inheritdoc />
    public bool HasClientDevice(string id)
    {
        return HasClientDevice(device => device.Id == id);
    }

    #endregion
}
