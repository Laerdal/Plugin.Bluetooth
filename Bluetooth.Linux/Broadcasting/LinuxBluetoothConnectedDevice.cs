namespace Bluetooth.Linux.Broadcasting;

/// <summary>
///     Linux stub for a connected GATT client device. Not yet supported via BlueZ.
/// </summary>
public class LinuxBluetoothConnectedDevice : BaseBluetoothConnectedDevice
{
    /// <inheritdoc />
    public LinuxBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster, string id, ILogger<IBluetoothConnectedDevice>? logger = null)
        : base(broadcaster, id, logger)
    {
    }

    /// <inheritdoc />
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        ValueTask.FromException(new NotSupportedException("Broadcasting is not yet supported on Linux via BlueZ."));
}
