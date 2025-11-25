namespace Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class BluetoothGattServerCallbackProxy
{
    /// <summary>
    /// Interface defining callback methods for Bluetooth GATT server events.
    /// Extends the base broadcaster interface with Android-specific server callback methods.
    /// </summary>
    public interface IBluetoothGattServerCallbackProxyDelegate : Bluetooth.Core.Abstractions.IBluetoothBroadcaster
    {
        /// <summary>
        /// Called when the MTU (Maximum Transmission Unit) for a connection changes.
        /// </summary>
        /// <param name="mtu">The new MTU size in bytes.</param>
        void OnMtuChanged(int mtu);

        /// <summary>
        /// Called to execute or cancel a pending write operation.
        /// </summary>
        /// <param name="requestId">An identifier for the write request.</param>
        /// <param name="execute">True to execute pending writes; false to cancel them.</param>
        void OnExecuteWrite(int requestId, bool execute);

        /// <summary>
        /// Called when a notification or indication has been sent to a remote device.
        /// </summary>
        /// <param name="status">The status of the notification operation.</param>
        void OnNotificationSent(GattStatus status);

        /// <summary>
        /// Called as a result of a PHY read operation.
        /// </summary>
        /// <param name="status">The status of the PHY read operation.</param>
        /// <param name="txPhy">The transmitter PHY in use.</param>
        /// <param name="rxPhy">The receiver PHY in use.</param>
        void OnPhyRead(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy);

        /// <summary>
        /// Called as a result of a PHY update operation.
        /// </summary>
        /// <param name="status">The status of the PHY update operation.</param>
        /// <param name="txPhy">The updated transmitter PHY in use.</param>
        /// <param name="rxPhy">The updated receiver PHY in use.</param>
        void OnPhyUpdate(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy);

        /// <summary>
        /// Called when a local service has been added to the GATT server.
        /// </summary>
        /// <param name="status">The status of the service addition operation.</param>
        /// <param name="service">The service that was added.</param>
        void OnServiceAdded(GattStatus status, BluetoothGattService? service);

        /// <summary>
        /// Called when a remote device has been connected or disconnected.
        /// </summary>
        /// <param name="status">The status of the connection state change.</param>
        /// <param name="newState">The new connection state.</param>
        void OnConnectionStateChange(ProfileState status, ProfileState newState);

        /// <summary>
        /// Called when a remote client requests to read a local characteristic.
        /// </summary>
        /// <param name="requestId">An identifier for the read request.</param>
        /// <param name="offset">The offset within the characteristic value to read from.</param>
        /// <param name="characteristic">The characteristic requested for reading.</param>
        void OnCharacteristicReadRequest(int requestId, int offset, BluetoothGattCharacteristic? characteristic);

        /// <summary>
        /// Called when a remote client requests to write to a local characteristic.
        /// </summary>
        /// <param name="requestId">An identifier for the write request.</param>
        /// <param name="characteristic">The characteristic requested for writing.</param>
        /// <param name="preparedWrite">True if this is a prepared write operation.</param>
        /// <param name="responseNeeded">True if a response is required for this write request.</param>
        /// <param name="offset">The offset within the characteristic value where the write should begin.</param>
        /// <param name="value">The value to be written to the characteristic.</param>
        void OnCharacteristicWriteRequest(int requestId,
            BluetoothGattCharacteristic? characteristic,
            bool preparedWrite,
            bool responseNeeded,
            int offset,
            byte[]? value);

        /// <summary>
        /// Called when a remote client requests to read a local descriptor.
        /// </summary>
        /// <param name="requestId">An identifier for the read request.</param>
        /// <param name="offset">The offset within the descriptor value to read from.</param>
        /// <param name="descriptor">The descriptor requested for reading.</param>
        void OnDescriptorReadRequest(int requestId, int offset, BluetoothGattDescriptor? descriptor);

        /// <summary>
        /// Called when a remote client requests to write to a local descriptor.
        /// </summary>
        /// <param name="requestId">An identifier for the write request.</param>
        /// <param name="descriptor">The descriptor requested for writing.</param>
        /// <param name="preparedWrite">True if this is a prepared write operation.</param>
        /// <param name="responseNeeded">True if a response is required for this write request.</param>
        /// <param name="offset">The offset within the descriptor value where the write should begin.</param>
        /// <param name="value">The value to be written to the descriptor.</param>
        void OnDescriptorWriteRequest(int requestId,
            BluetoothGattDescriptor? descriptor,
            bool preparedWrite,
            bool responseNeeded,
            int offset,
            byte[]? value);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
