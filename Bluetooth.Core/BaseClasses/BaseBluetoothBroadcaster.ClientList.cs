namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothBroadcaster
{
    /// <inheritdoc/>
    public event EventHandler<ClientConnectedEventArgs>? ClientConnected;

    /// <inheritdoc/>
    public event EventHandler<ClientDisconnectedEventArgs>? ClientDisconnected;

    /// <inheritdoc/>
    public int MaximumConnectedClients
    {
        get => GetValue(7); // Default to 7, common maximum for BLE peripherals
        set => SetValue(value);
    }

    /// <inheritdoc/>
    public IEnumerable<string> ConnectedClients
    {
        get
        {
            lock (ConnectedClientsInternal)
            {
                return ConnectedClientsInternal.Keys.ToList();
            }
        }
    }

    /// <inheritdoc/>
    public int ConnectedClientCount
    {
        get
        {
            lock (ConnectedClientsInternal)
            {
                return ConnectedClientsInternal.Count;
            }
        }
    }

    /// <summary>
    /// Internal dictionary mapping client IDs to their device information.
    /// </summary>
    protected Dictionary<string, IBluetoothDevice> ConnectedClientsInternal { get; } = new();

    /// <inheritdoc/>
    public IBluetoothDevice? GetConnectedClientOrDefault(string clientId)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        lock (ConnectedClientsInternal)
        {
            return ConnectedClientsInternal.GetValueOrDefault(clientId);
        }
    }

    /// <summary>
    /// Called when a client connects to the broadcaster.
    /// </summary>
    /// <param name="clientId">The unique identifier of the connected client.</param>
    /// <param name="device">The device information for the connected client.</param>
    /// <param name="clientName">The name of the connected client, if available.</param>
    protected virtual void OnClientConnected(string clientId, IBluetoothDevice device, string? clientName = null)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(device);

        lock (ConnectedClientsInternal)
        {
            ConnectedClientsInternal[clientId] = device;
        }

        ClientConnected?.Invoke(this, new ClientConnectedEventArgs(clientId, clientName));
    }

    /// <summary>
    /// Called when a client disconnects from the broadcaster.
    /// </summary>
    /// <param name="clientId">The unique identifier of the disconnected client.</param>
    /// <param name="clientName">The name of the disconnected client, if available.</param>
    protected virtual void OnClientDisconnected(string clientId, string? clientName = null)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        lock (ConnectedClientsInternal)
        {
            ConnectedClientsInternal.Remove(clientId);
        }

        ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(clientId, clientName));
    }
}
