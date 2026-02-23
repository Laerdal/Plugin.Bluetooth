namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc />
public class DotNetCoreBluetoothConnectedDevice : BaseBluetoothConnectedDevice
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster,
        IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec request,
        ILogger<IBluetoothConnectedDevice>? logger = null) : base(broadcaster, request, logger)
    {
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
