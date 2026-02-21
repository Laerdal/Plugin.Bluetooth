namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
///     Event arguments for the client connection state changed event.
/// </summary>
public class ClientConnectionStateChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ClientConnectionStateChangedEventArgs" /> class.
    /// </summary>
    /// <param name="device">The client device whose connection state changed.</param>
    /// <param name="isConnected">Indicates whether the client is now connected.</param>
    public ClientConnectionStateChangedEventArgs(IBluetoothConnectedDevice device, bool isConnected)
    {
        Device = device;
        IsConnected = isConnected;
    }

    /// <summary>
    ///     Gets the client device whose connection state changed.
    /// </summary>
    public IBluetoothConnectedDevice Device { get; }

    /// <summary>
    ///     Gets a value indicating whether the client is now connected.
    /// </summary>
    public bool IsConnected { get; }
}