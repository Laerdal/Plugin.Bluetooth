namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple-specific factory spec for creating <see cref="AppleBluetoothRemoteDevice" /> instances.
///     Extends the base spec with the native <see cref="CBPeripheral" /> required for CoreBluetooth.
/// </summary>
public record AppleBluetoothRemoteDeviceFactorySpec(string DeviceId, Manufacturer Manufacturer, CBPeripheral CbPeripheral)
    : IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec(DeviceId, Manufacturer)
{
    /// <summary>
    ///     Initializes a new instance from an Apple Bluetooth advertisement.
    /// </summary>
    /// <param name="advertisement">The Apple advertisement containing the peripheral and device info.</param>
    public AppleBluetoothRemoteDeviceFactorySpec(AppleBluetoothAdvertisement advertisement)
        : this(advertisement.BluetoothAddress, advertisement.Manufacturer, advertisement.CbPeripheral)
    {
    }
}
