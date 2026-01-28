namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public sealed partial class CbPeripheralWrapper
{
    /// <summary>
    /// Delegate interface for CoreBluetooth peripheral proxy callbacks, extending the base Bluetooth device interface.
    /// </summary>
    public interface ICbPeripheralDelegate : IBluetoothDevice
    {
        /// <summary>
        /// Called when a service is discovered on the peripheral.
        /// </summary>
        /// <param name="error">The error that occurred during service discovery, or null if successful.</param>
        void DiscoveredService(NSError? error);

        /// <summary>
        /// Called when the RSSI (Received Signal Strength Indicator) value is read.
        /// </summary>
        /// <param name="error">The error that occurred during RSSI reading, or null if successful.</param>
        /// <param name="rssi">The RSSI value in decibels.</param>
        void RssiRead(NSError? error, NSNumber rssi);

        /// <summary>
        /// Called when the RSSI value is updated.
        /// </summary>
        /// <param name="error">The error that occurred during RSSI update, or null if successful.</param>
        void RssiUpdated(NSError? error);

        /// <summary>
        /// Called when the peripheral's name is updated.
        /// </summary>
        void UpdatedName();

        /// <summary>
        /// Called when an L2CAP channel is opened.
        /// </summary>
        /// <param name="error">The error that occurred during channel opening, or null if successful.</param>
        /// <param name="channel">The L2CAP channel that was opened, or null if failed.</param>
        void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel);

        /// <summary>
        /// Called when the peripheral is ready to send write operations without response.
        /// </summary>
        void IsReadyToSendWriteWithoutResponse();

        /// <summary>
        /// Called when services on the peripheral are modified.
        /// </summary>
        /// <param name="services">The array of services that were modified.</param>
        void ModifiedServices(CBService[] services);

        /// <summary>
        /// Gets the service delegate for the specified CoreBluetooth service.
        /// </summary>
        /// <param name="characteristicService">The CoreBluetooth service to get the delegate for.</param>
        /// <returns>The service delegate for the specified service.</returns>
        ICbServiceDelegate GetService(CBService? characteristicService);
    }
}
#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords
