namespace Bluetooth.Core.EventArgs;

/// <summary>
/// Event arguments for when a client connects to the broadcaster.
/// </summary>
public class ClientConnectedEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientConnectedEventArgs" /> class.
    /// </summary>
    /// <param name="clientId">The unique identifier of the connected client.</param>
    /// <param name="clientName">The name of the connected client, if available.</param>
    public ClientConnectedEventArgs(string clientId, string? clientName = null)
    {
        ClientId = clientId;
        ClientName = clientName;
    }

    /// <summary>
    /// Gets the unique identifier of the connected client.
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// Gets the name of the connected client, if available.
    /// </summary>
    public string? ClientName { get; }
}
