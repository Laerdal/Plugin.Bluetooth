namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc/>
public class DotNetCoreBluetoothLocalCharacteristic : Core.Broadcasting.BaseBluetoothLocalCharacteristic
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothLocalCharacteristic(IBluetoothLocalService localService,
        IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec,
        IBluetoothLocalDescriptorFactory localDescriptorRepository,
        ILogger<IBluetoothLocalCharacteristic>? logger = null) : base(localService, spec, localDescriptorRepository, logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected async override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
