namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Event arguments for the pairing state changed event.
/// </summary>
public class PairingStateChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PairingStateChangedEventArgs" /> class.
    /// </summary>
    /// <param name="device">The device whose pairing state changed.</param>
    /// <param name="isPaired">Indicates whether the device is now paired.</param>
    public PairingStateChangedEventArgs(IBluetoothRemoteDevice device, bool isPaired)
    {
        Device = device;
        IsPaired = isPaired;
    }

    /// <summary>
    ///     Gets the device whose pairing state changed.
    /// </summary>
    public IBluetoothRemoteDevice Device { get; }

    /// <summary>
    ///     Gets a value indicating whether the device is now paired.
    /// </summary>
    public bool IsPaired { get; }
}
