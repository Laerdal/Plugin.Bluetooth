namespace Bluetooth.Maui;

public record BluetoothDeviceConnectionOptions : BaseBluetoothDeviceConnectionOptions
{
    /// <summary>
    /// Gets or sets the iOS-specific peripheral connection options used when connecting to the device.
    /// </summary>
    public PeripheralConnectionOptions? PeripheralConnectionOptions { get; init; } = new PeripheralConnectionOptions()
    {
        NotifyOnConnection = true,
        NotifyOnDisconnection = true,
        NotifyOnNotification = true
    };
}
