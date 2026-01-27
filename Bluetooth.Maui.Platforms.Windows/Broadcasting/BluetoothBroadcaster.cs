using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Windows.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcaster : BaseBluetoothBroadcaster
{
    /// <inheritdoc/>
    public BluetoothBroadcaster(IBluetoothAdapter adapter,
        IBluetoothBroadcastServiceFactory serviceFactory,
        IBluetoothBroadcastClientDeviceFactory deviceFactory,
        IBluetoothPermissionManager permissionManager,
        ILogger? logger = null) : base(adapter,
                                       serviceFactory,
                                       deviceFactory,
                                       permissionManager,
                                       logger)
    {
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStartAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
