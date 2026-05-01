namespace Bluetooth.Linux.Broadcasting;

/// <summary>
///     Linux stub for a local GATT descriptor. Not yet supported via BlueZ.
/// </summary>
public class LinuxBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor
{
    /// <inheritdoc />
    public LinuxBluetoothLocalDescriptor(IBluetoothLocalCharacteristic characteristic, Guid id, ReadOnlyMemory<byte>? initialValue = null, string? name = null, ILogger<IBluetoothLocalDescriptor>? logger = null)
        : base(characteristic, id, initialValue, name, logger)
    {
    }

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken) =>
        ValueTask.FromException(new NotSupportedException("Broadcasting is not yet supported on Linux via BlueZ."));
}
