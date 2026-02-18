using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Abstractions.Broadcasting.Options;
using Bluetooth.Core.Infrastructure.Scheduling;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Windows.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcaster : BaseBluetoothBroadcaster
{
    /// <inheritdoc/>
    public BluetoothBroadcaster(
        IBluetoothAdapter adapter,
        IBluetoothLocalServiceFactory localServiceFactory,
        IBluetoothConnectedDeviceFactory connectedDeviceFactory,
        IBluetoothPermissionManager permissionManager,
        ITicker ticker,
        ILogger<IBluetoothBroadcaster>? logger = null)
        : base(adapter, localServiceFactory, connectedDeviceFactory, permissionManager, ticker, logger)
    {
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException("BluetoothBroadcaster is not yet implemented on Windows.");
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("BluetoothBroadcaster is not yet implemented on Windows.");
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("BluetoothBroadcaster is not yet implemented on Windows.");
    }
}
