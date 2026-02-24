namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <summary>
///     Windows-specific implementation of the Bluetooth device factory spec.
/// </summary>
public record WindowsBluetoothRemoteDeviceFactorySpec : IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothRemoteDeviceFactorySpec" /> record from a Windows Bluetooth advertisement.
    /// </summary>
    /// <param name="advertisement">The Windows Bluetooth advertisement containing the device information.</param>
    public WindowsBluetoothRemoteDeviceFactorySpec(WindowsBluetoothAdvertisement advertisement)
        : base(advertisement)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothRemoteDeviceFactorySpec" /> record with device ID and manufacturer.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the Bluetooth device (Bluetooth address).</param>
    /// <param name="manufacturer">The manufacturer information of the Bluetooth device.</param>
    public WindowsBluetoothRemoteDeviceFactorySpec(string deviceId, Manufacturer manufacturer)
        : base(deviceId, manufacturer)
    {
    }
}
