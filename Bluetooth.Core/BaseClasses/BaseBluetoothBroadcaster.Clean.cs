namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothBroadcaster
{
    /// <inheritdoc/>
    public async ValueTask CleanAsync(string clientId)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        if (GetConnectedClientOrDefault(clientId) != null)
        {
            await DisconnectClientAsync(clientId).ConfigureAwait(false);
        }

        lock (ConnectedClientsInternal)
        {
            ConnectedClientsInternal.Remove(clientId);
        }
    }

    /// <inheritdoc/>
    public async ValueTask CleanAsync()
    {
        // Disconnect all clients
        var clients = ConnectedClients.ToList();
        foreach (var clientId in clients)
        {
            await CleanAsync(clientId).ConfigureAwait(false);
        }

        // Remove all services
        await RemoveAllServicesAsync().ConfigureAwait(false);

        // Clear advertisement data
        ClearAdvertisementData();
    }
}
