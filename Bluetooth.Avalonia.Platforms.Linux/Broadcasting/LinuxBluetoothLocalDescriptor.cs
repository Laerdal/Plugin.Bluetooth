namespace Bluetooth.Avalonia.Platforms.Linux.Broadcasting;

/// <inheritdoc />
public class LinuxBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public LinuxBluetoothLocalDescriptor(IBluetoothLocalCharacteristic characteristic,
        Guid id,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalDescriptor>? logger = null) : base(characteristic,
                                                                  id,
                                                                  initialValue,
                                                                  name,
                                                                  logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }
}
