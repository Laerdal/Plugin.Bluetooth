namespace Bluetooth.Linux.Broadcasting;

/// <summary>
///     Linux stub for a local GATT characteristic. Not yet supported via BlueZ.
/// </summary>
public class LinuxBluetoothLocalCharacteristic : BaseBluetoothLocalCharacteristic
{
    /// <inheritdoc />
    public LinuxBluetoothLocalCharacteristic(IBluetoothLocalService service, Guid id, BluetoothCharacteristicProperties properties, BluetoothCharacteristicPermissions permissions, ReadOnlyMemory<byte>? initialValue = null, string? name = null, ILogger<IBluetoothLocalCharacteristic>? logger = null)
        : base(service, id, properties, permissions, initialValue, name, logger)
    {
    }

    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalDescriptor> NativeCreateDescriptorAsync(Guid id, string? name = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        ValueTask.FromException<IBluetoothLocalDescriptor>(new NotSupportedException("Broadcasting is not yet supported on Linux via BlueZ."));

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        ValueTask.FromException(new NotSupportedException("Broadcasting is not yet supported on Linux via BlueZ."));
}
