namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc />
public class DotNetCoreBluetoothLocalService : BaseBluetoothLocalService
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothLocalService(IBluetoothBroadcaster broadcaster,
        Guid id,
        string? name = null,
        bool isPrimary = true,
        ILogger<IBluetoothLocalService>? logger = null) : base(broadcaster,
                                                               id,
                                                               name,
                                                               isPrimary,
                                                               logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<IBluetoothLocalCharacteristic> NativeCreateCharacteristicAsync(Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        string? name = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
