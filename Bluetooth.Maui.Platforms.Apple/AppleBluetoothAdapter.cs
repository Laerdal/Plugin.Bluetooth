namespace Bluetooth.Maui.Platforms.Apple;

/// <summary>
///     iOS implementation of the Bluetooth adapter using Core Bluetooth framework.
/// </summary>
public class AppleBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    public AppleBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}
