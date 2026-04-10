namespace Bluetooth.Avalonia.Platforms.Windows;

/// <summary>
///     Windows implementation of the Bluetooth adapter for Avalonia applications.
/// </summary>
/// <remarks>
///     Actual BLE adapter logic (wrapping WinRT <c>BluetoothAdapter</c> / <c>Radio</c>)
///     is to be implemented here, mirroring the pattern in
///     <c>Bluetooth.Maui.Platforms.Win.WindowsBluetoothAdapter</c>.
/// </remarks>
public class WindowsBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    protected WindowsBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}
