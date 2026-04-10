namespace Bluetooth.Avalonia.Platforms.Apple.Scanning;

/// <summary>
///     Factory for creating Apple (iOS / macOS native) Bluetooth remote device instances.
/// </summary>
/// <remarks>
///     This is a scaffold stub. The actual factory implementation needs to be ported from
///     <c>Bluetooth.Maui.Platforms.Apple</c>.
/// </remarks>
public class AppleBluetoothRemoteDeviceFactory : IBluetoothRemoteDeviceFactory
{
    /// <inheritdoc />
    /// <exception cref="NotImplementedException">Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.</exception>
    public IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothAdvertisement advertisement)
    {
        throw new NotImplementedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException">Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.</exception>
    public IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer)
    {
        throw new NotImplementedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }
}
