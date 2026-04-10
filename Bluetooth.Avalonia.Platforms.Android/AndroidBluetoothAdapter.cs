namespace Bluetooth.Avalonia.Platforms.Android;

/// <summary>
///     Android implementation of the Bluetooth adapter for Avalonia applications.
/// </summary>
/// <remarks>
///     The actual BLE adapter logic (wrapping <c>BluetoothManager</c> / <c>BluetoothAdapter</c>)
///     is to be implemented here, mirroring the pattern in
///     <c>Bluetooth.Maui.Platforms.Droid.AndroidBluetoothAdapter</c>.
/// </remarks>
public class AndroidBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    protected AndroidBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}
