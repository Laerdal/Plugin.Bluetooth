namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc/>
public class BluetoothLocalCharacteristic : Core.Broadcasting.BaseBluetoothLocalCharacteristic
{
    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothLocalCharacteristic(IBluetoothLocalService localService,
        IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec request,
        IBluetoothLocalDescriptorFactory localDescriptorRepository,
        ILogger<IBluetoothLocalCharacteristic>? logger = null) : base(localService, request, localDescriptorRepository, logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected async override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
