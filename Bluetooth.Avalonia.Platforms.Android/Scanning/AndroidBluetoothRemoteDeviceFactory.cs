namespace Bluetooth.Avalonia.Platforms.Android.Scanning;

/// <summary>
///     Factory for creating Android Bluetooth remote device instances.
/// </summary>
/// <remarks>
///     This is a scaffold stub. The actual factory implementation needs to be ported from
///     <c>Bluetooth.Maui.Platforms.Droid</c>.
/// </remarks>
public class AndroidBluetoothRemoteDeviceFactory : IBluetoothRemoteDeviceFactory
{
    /// <inheritdoc />
    /// <exception cref="NotImplementedException">Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.</exception>
    public IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothAdvertisement advertisement)
    {
        throw new NotImplementedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException">Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.</exception>
    public IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer)
    {
        throw new NotImplementedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }
}
