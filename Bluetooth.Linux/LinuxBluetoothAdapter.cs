namespace Bluetooth.Linux;

/// <summary>
///     Linux implementation of the Bluetooth adapter using BlueZ via D-Bus.
/// </summary>
public class LinuxBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    public LinuxBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}
