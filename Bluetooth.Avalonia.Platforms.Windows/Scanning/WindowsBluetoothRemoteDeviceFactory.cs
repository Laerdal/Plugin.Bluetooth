namespace Bluetooth.Avalonia.Platforms.Windows.Scanning;

/// <summary>
///     Factory for creating Windows Bluetooth remote device instances.
/// </summary>
public class WindowsBluetoothRemoteDeviceFactory : IBluetoothRemoteDeviceFactory
{
    /// <inheritdoc />
    /// <exception cref="NotImplementedException">Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.</exception>
    public IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothAdvertisement advertisement)
    {
        throw new NotImplementedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException">Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.</exception>
    public IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer)
    {
        throw new NotImplementedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }
}
