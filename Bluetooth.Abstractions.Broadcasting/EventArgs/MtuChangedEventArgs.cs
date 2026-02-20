namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
///     Event arguments for the MTU changed event for a broadcast client device.
/// </summary>
public class MtuChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MtuChangedEventArgs" /> class.
    /// </summary>
    /// <param name="device">The client device whose MTU changed.</param>
    /// <param name="mtu">The new MTU value.</param>
    public MtuChangedEventArgs(IBluetoothConnectedDevice device, int mtu)
    {
        Device = device;
        Mtu = mtu;
    }

    /// <summary>
    ///     Gets the client device whose MTU changed.
    /// </summary>
    public IBluetoothConnectedDevice Device { get; }

    /// <summary>
    ///     Gets the new MTU value.
    /// </summary>
    public int Mtu { get; }
}