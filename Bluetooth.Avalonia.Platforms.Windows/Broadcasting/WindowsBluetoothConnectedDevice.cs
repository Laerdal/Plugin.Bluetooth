namespace Bluetooth.Avalonia.Platforms.Windows.Broadcasting;

/// <inheritdoc />
public class WindowsBluetoothConnectedDevice : BaseBluetoothConnectedDevice
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public WindowsBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster, string id, ILogger<IBluetoothConnectedDevice>? logger = null) : base(broadcaster, id, logger)
    {
        throw new PlatformNotSupportedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }
}
