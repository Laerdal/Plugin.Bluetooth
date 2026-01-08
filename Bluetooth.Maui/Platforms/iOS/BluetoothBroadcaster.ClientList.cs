using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, as a peripheral, we cannot explicitly disconnect a central device.
    /// Centrals must disconnect themselves. This method does nothing but is provided for API compatibility.
    /// </remarks>
    protected override Task NativeDisconnectClientAsync(string clientId, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        // iOS peripheral managers cannot force disconnect centrals
        // The central must disconnect itself
        // We just remove it from our tracking
        return Task.CompletedTask;
    }
}
