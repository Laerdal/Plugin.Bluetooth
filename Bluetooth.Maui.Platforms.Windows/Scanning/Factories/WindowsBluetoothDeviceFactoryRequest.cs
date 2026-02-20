namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <summary>
///     Windows-specific implementation of the Bluetooth device factory request.
/// </summary>
public record WindowsBluetoothDeviceFactoryRequest : IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothDeviceFactoryRequest" /> record from a Windows Bluetooth advertisement.
    /// </summary>
    /// <param name="advertisement">The Windows Bluetooth advertisement containing the device information.</param>
    public WindowsBluetoothDeviceFactoryRequest(WindowsBluetoothAdvertisement advertisement)
        : base(advertisement)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothDeviceFactoryRequest" /> record with device ID and manufacturer.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the Bluetooth device (Bluetooth address).</param>
    /// <param name="manufacturer">The manufacturer information of the Bluetooth device.</param>
    public WindowsBluetoothDeviceFactoryRequest(string deviceId, Manufacturer manufacturer)
        : base(deviceId, manufacturer)
    {
    }
}
