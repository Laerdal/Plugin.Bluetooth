namespace Bluetooth.Linux.Broadcasting;

/// <summary>
///     Linux stub for a local GATT service. Not yet supported via BlueZ.
/// </summary>
public class LinuxBluetoothLocalService : BaseBluetoothLocalService
{
    /// <inheritdoc />
    public LinuxBluetoothLocalService(IBluetoothBroadcaster broadcaster, Guid id, string? name = null, bool isPrimary = true, ILogger<IBluetoothLocalService>? logger = null)
        : base(broadcaster, id, name, isPrimary, logger)
    {
    }

    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalCharacteristic> NativeCreateCharacteristicAsync(Guid id, BluetoothCharacteristicProperties properties, BluetoothCharacteristicPermissions permissions, string? name = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        ValueTask.FromException<IBluetoothLocalCharacteristic>(new NotSupportedException("Broadcasting is not yet supported on Linux via BlueZ."));
}
