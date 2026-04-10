namespace Bluetooth.Avalonia.Platforms.Linux.Broadcasting;

/// <inheritdoc />
public class LinuxBluetoothConnectedDevice : BaseBluetoothConnectedDevice
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public LinuxBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster, string id, ILogger<IBluetoothConnectedDevice>? logger = null) : base(broadcaster, id, logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }
}
