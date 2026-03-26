namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc />
public class DotNetCoreBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothLocalDescriptor(IBluetoothLocalCharacteristic characteristic,
        Guid id,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalDescriptor>? logger = null) : base(characteristic,
                                                                  id,
                                                                  initialValue,
                                                                  name,
                                                                  logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
