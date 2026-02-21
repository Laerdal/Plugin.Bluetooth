namespace Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class GattSessionWrapper
{
    /// <summary>
    ///     Delegate interface for handling GATT session operations and events.
    ///     Extends the base Bluetooth device interface with Windows-specific GATT session callbacks.
    /// </summary>
    public interface IGattSessionDelegate : IBluetoothRemoteDevice
    {
        /// <summary>
        ///     Called when the GATT session status changes.
        /// </summary>
        /// <param name="argsStatus">The new GATT session status.</param>
        void OnGattSessionStatusChanged(GattSessionStatus argsStatus);

        /// <summary>
        ///     Called when the maximum PDU (Protocol Data Unit) size changes.
        /// </summary>
        void OnMaxPduSizeChanged();
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix