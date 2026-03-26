namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc/>
public class DotNetCoreBluetoothLocalCharacteristic : Core.Broadcasting.BaseBluetoothLocalCharacteristic
{

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothLocalCharacteristic(IBluetoothLocalService service,
        Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalCharacteristic>? logger = null) : base(service,
                                                                      id,
                                                                      properties,
                                                                      permissions,
                                                                      initialValue,
                                                                      name,
                                                                      logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<IBluetoothLocalDescriptor> NativeCreateDescriptorAsync(Guid id, string? name = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
