namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Event arguments for the L2CAP data received event.
/// </summary>
public class L2CapDataReceivedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="L2CapDataReceivedEventArgs" /> class.
    /// </summary>
    /// <param name="channel">The L2CAP channel that received the data.</param>
    /// <param name="data">The data that was received.</param>
    public L2CapDataReceivedEventArgs(IBluetoothL2CapChannel channel, ReadOnlyMemory<byte> data)
    {
        Channel = channel;
        Data = data;
    }

    /// <summary>
    ///     Gets the L2CAP channel that received the data.
    /// </summary>
    public IBluetoothL2CapChannel Channel { get; }

    /// <summary>
    ///     Gets the data that was received.
    /// </summary>
    public ReadOnlyMemory<byte> Data { get; }
}