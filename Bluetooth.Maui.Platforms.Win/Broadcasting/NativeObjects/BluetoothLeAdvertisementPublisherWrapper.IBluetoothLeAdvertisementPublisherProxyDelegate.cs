namespace Bluetooth.Maui.Platforms.Win.Broadcasting.NativeObjects;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class BluetoothLeAdvertisementPublisherWrapper
{
    /// <summary>
    ///     Delegate interface for handling Bluetooth LE advertisement publisher operations and events.
    ///     Extends the base Bluetooth broadcaster interface with Windows-specific publisher callbacks.
    /// </summary>
    public interface IBluetoothLeAdvertisementPublisherProxyDelegate
    {
        /// <summary>
        ///     Called when the advertisement publisher status changes.
        /// </summary>
        /// <param name="status">The new publisher status.</param>
        /// <param name="errorCode">The error code associated with the status change, if any.</param>
        /// <param name="selectedTransmitPowerLevelInDBm">The selected transmit power level in dBm, if applicable.</param>
        void OnAdvertisementPublisherStatusChanged(BluetoothLEAdvertisementPublisherStatus status, BluetoothError errorCode, short? selectedTransmitPowerLevelInDBm = null);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
