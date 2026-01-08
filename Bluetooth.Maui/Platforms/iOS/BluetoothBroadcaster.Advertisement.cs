using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, when advertisement data changes while advertising, we need to stop and restart advertising
    /// with the new data. This is a limitation of the Core Bluetooth framework.
    /// </remarks>
    protected override void NativeAdvertisementDataChanged()
    {
        // If we're currently advertising, we need to restart advertising with the new data
        // iOS doesn't support updating advertisement data while advertising is active
        if (IsRunning)
        {
            // The data will be picked up on next start
            // Note: Actual restart would need to be coordinated with the start/stop mechanism
        }
    }
}
