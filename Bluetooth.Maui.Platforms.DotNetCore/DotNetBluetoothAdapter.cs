namespace Bluetooth.Maui.Platforms.DotNetCore;

/// <summary>
///     Android implementation of the Bluetooth adapter.
/// </summary>
public abstract class DotNetBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    protected DotNetBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}