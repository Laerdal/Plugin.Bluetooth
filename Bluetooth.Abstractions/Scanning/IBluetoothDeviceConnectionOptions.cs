namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Represents Bluetooth device connection options.
/// </summary>
public interface IBluetoothDeviceConnectionOptions
{
    /// <summary>
    /// Gets a value indicating whether to wait for an advertisement before connecting to the device.
    /// </summary>
    bool WaitForAdvertisementBeforeConnecting { get; }
}
