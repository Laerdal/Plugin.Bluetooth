namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
/// Event arguments for the RSSI changed event during an active connection.
/// </summary>
public class RssiChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RssiChangedEventArgs"/> class.
    /// </summary>
    /// <param name="device">The device whose RSSI changed.</param>
    /// <param name="rssi">The new RSSI value.</param>
    public RssiChangedEventArgs(IBluetoothRemoteDevice device, int rssi)
    {
        Device = device;
        Rssi = rssi;
    }

    /// <summary>
    /// Gets the device whose RSSI changed.
    /// </summary>
    public IBluetoothRemoteDevice Device { get; }

    /// <summary>
    /// Gets the new RSSI value.
    /// </summary>
    public int Rssi { get; }
}
