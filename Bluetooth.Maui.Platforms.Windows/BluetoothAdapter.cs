using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Windows;

/// <summary>
/// Android implementation of the Bluetooth adapter.
/// </summary>
public abstract class BluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc/>
    protected BluetoothAdapter(ILogger? logger = null) : base(logger)
    {
    }
}
