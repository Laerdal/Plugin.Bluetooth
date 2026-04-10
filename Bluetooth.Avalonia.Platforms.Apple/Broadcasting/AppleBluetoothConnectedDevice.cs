namespace Bluetooth.Avalonia.Platforms.Apple.Broadcasting;

/// <inheritdoc />
public class AppleBluetoothConnectedDevice : BaseBluetoothConnectedDevice
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AppleBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster, string id, ILogger<IBluetoothConnectedDevice>? logger = null) : base(broadcaster, id, logger)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }
}
