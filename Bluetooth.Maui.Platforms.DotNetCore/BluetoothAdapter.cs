using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.DotNetCore;

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
