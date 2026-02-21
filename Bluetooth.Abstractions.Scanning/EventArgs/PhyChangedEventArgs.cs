namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Event arguments for the PHY (Physical Layer) changed event.
/// </summary>
public class PhyChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PhyChangedEventArgs" /> class.
    /// </summary>
    /// <param name="device">The device whose PHY changed.</param>
    /// <param name="txPhy">The transmit PHY mode.</param>
    /// <param name="rxPhy">The receive PHY mode.</param>
    public PhyChangedEventArgs(IBluetoothRemoteDevice device, PhyMode txPhy, PhyMode rxPhy)
    {
        Device = device;
        TxPhy = txPhy;
        RxPhy = rxPhy;
    }

    /// <summary>
    ///     Gets the device whose PHY changed.
    /// </summary>
    public IBluetoothRemoteDevice Device { get; }

    /// <summary>
    ///     Gets the transmit PHY mode.
    /// </summary>
    public PhyMode TxPhy { get; }

    /// <summary>
    ///     Gets the receive PHY mode.
    /// </summary>
    public PhyMode RxPhy { get; }
}