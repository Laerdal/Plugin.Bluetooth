namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Event arguments for the device connection state changed event.
/// </summary>
public class DeviceConnectionStateChangedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceConnectionStateChangedEventArgs" /> class.
    /// </summary>
    /// <param name="device">The device whose connection state changed.</param>
    /// <param name="isConnected">Indicates whether the device is now connected.</param>
    public DeviceConnectionStateChangedEventArgs(IBluetoothRemoteDevice device, bool isConnected)
    {
        Device = device;
        IsConnected = isConnected;
    }

    /// <summary>
    ///     Gets the device whose connection state changed.
    /// </summary>
    public IBluetoothRemoteDevice Device { get; }

    /// <summary>
    ///     Gets a value indicating whether the device is now connected.
    /// </summary>
    public bool IsConnected { get; }
}
