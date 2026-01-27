namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
/// Event arguments for when a client disconnects from the broadcaster.
/// </summary>
public class ClientDisconnectedEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientDisconnectedEventArgs" /> class.
    /// </summary>
    /// <param name="clientId">The unique identifier of the disconnected client.</param>
    /// <param name="clientName">The name of the disconnected client, if available.</param>
    public ClientDisconnectedEventArgs(string clientId, string? clientName = null)
    {
        ClientId = clientId;
        ClientName = clientName;
    }

    /// <summary>
    /// Gets the unique identifier of the disconnected client.
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// Gets the name of the disconnected client, if available.
    /// </summary>
    public string? ClientName { get; }
}
