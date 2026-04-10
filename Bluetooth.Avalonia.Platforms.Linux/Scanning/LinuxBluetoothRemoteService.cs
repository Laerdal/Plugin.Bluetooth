namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <inheritdoc />
public class LinuxBluetoothRemoteService : BaseBluetoothRemoteService
{

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public LinuxBluetoothRemoteService(IBluetoothRemoteDevice parentDevice,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null) : base(parentDevice, id, nameProvider, logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }
}
