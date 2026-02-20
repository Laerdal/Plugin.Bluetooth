namespace Bluetooth.Maui.Platforms.Windows.Broadcasting;

/// <inheritdoc />
public class WindowsBluetoothBroadcaster : BaseBluetoothBroadcaster
{
    /// <inheritdoc />
    public WindowsBluetoothBroadcaster(
        IBluetoothAdapter adapter,
        IBluetoothLocalServiceFactory localServiceFactory,
        IBluetoothConnectedDeviceFactory connectedDeviceFactory,
        IBluetoothPermissionManager permissionManager,
        ITicker ticker,
        ILogger<IBluetoothBroadcaster>? logger = null)
        : base(adapter, localServiceFactory, connectedDeviceFactory, permissionManager, ticker, logger)
    {
    }

    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException("WindowsBluetoothBroadcaster is not yet implemented on Windows.");
    }

    /// <inheritdoc />
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("WindowsBluetoothBroadcaster is not yet implemented on Windows.");
    }

    /// <inheritdoc />
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("WindowsBluetoothBroadcaster is not yet implemented on Windows.");
    }
}