namespace Bluetooth.Maui.Platforms.Windows.Scanning.NativeObjects;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class BluetoothLeAdvertisementWatcherWrapper
{
    /// <summary>
    /// Delegate interface for handling Bluetooth LE advertisement watcher operations and events.
    /// Extends the base Bluetooth scanner interface with Windows-specific watcher callbacks.
    /// </summary>
    public interface IBluetoothLeAdvertisementWatcherProxyDelegate : IBluetoothScanner
    {
        /// <summary>
        /// Called when the advertisement watcher stops due to an error or state change.
        /// </summary>
        /// <param name="argsError">The error that caused the watcher to stop, if any.</param>
        void OnAdvertisementWatcherStopped(BluetoothError argsError);

        /// <summary>
        /// Called when a Bluetooth LE advertisement is received.
        /// </summary>
        /// <param name="argsAdvertisement">The advertisement event arguments containing device and advertisement data.</param>
        void OnAdvertisementReceived(BluetoothLEAdvertisementReceivedEventArgs argsAdvertisement);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
