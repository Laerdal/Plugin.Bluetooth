

namespace Bluetooth.Maui.Platforms.Apple;

/// <summary>
/// iOS implementation of the Bluetooth adapter using Core Bluetooth framework.
/// </summary>
public class BluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc/>
    public BluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}
