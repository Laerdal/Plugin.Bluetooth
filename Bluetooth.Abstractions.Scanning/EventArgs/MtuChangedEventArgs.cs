namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Event arguments for the MTU changed event.
/// </summary>
public class MtuChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MtuChangedEventArgs" /> class.
    /// </summary>
    /// <param name="device">The device whose MTU changed.</param>
    /// <param name="mtu">The new MTU value.</param>
    public MtuChangedEventArgs(IBluetoothRemoteDevice device, int mtu)
    {
        Device = device;
        Mtu = mtu;
    }

    /// <summary>
    ///     Gets the device whose MTU changed.
    /// </summary>
    public IBluetoothRemoteDevice Device { get; }

    /// <summary>
    ///     Gets the new MTU value.
    /// </summary>
    public int Mtu { get; }
}