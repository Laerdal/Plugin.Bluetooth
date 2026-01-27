namespace Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class BluetoothLeAdvertisementPublisherWrapper
{
    /// <summary>
    /// Delegate interface for handling Bluetooth LE advertisement publisher operations and events.
    /// Extends the base Bluetooth broadcaster interface with Windows-specific publisher callbacks.
    /// </summary>
    public interface IBluetoothLeAdvertisementPublisherProxyDelegate : IBluetoothBroadcaster
    {
        /// <summary>
        /// Called when the advertisement publisher status changes.
        /// </summary>
        /// <param name="status">The new publisher status.</param>
        void OnAdvertisementPublisherStatusChanged(BluetoothLEAdvertisementPublisherStatus status);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
