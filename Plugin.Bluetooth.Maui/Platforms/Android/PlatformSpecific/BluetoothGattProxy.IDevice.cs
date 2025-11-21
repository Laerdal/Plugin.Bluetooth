namespace Plugin.Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible

public partial class BluetoothGattProxy
{
    /// <summary>
    /// Interface for handling Bluetooth GATT device operations.
    /// Extends the base device interface with Android-specific callback methods.
    /// </summary>
    public interface IDevice : Plugin.Bluetooth.Abstractions.IBluetoothDevice
    {
        /// <summary>
        /// Gets the service wrapper for the specified GATT service.
        /// </summary>
        /// <param name="nativeService">The GATT service to get a wrapper for.</param>
        /// <returns>The service wrapper instance.</returns>
        public IService GetService(BluetoothGattService? nativeService);

        /// <summary>
        /// Called when the connection state changes.
        /// </summary>
        /// <param name="status">The status of the connection state change.</param>
        /// <param name="newState">The new connection state.</param>
        public void OnConnectionStateChange(GattStatus status, ProfileState newState);

        /// <summary>
        /// Called when service discovery has completed.
        /// </summary>
        /// <param name="status">The status of the service discovery operation.</param>
        public void OnServicesDiscovered(GattStatus status);

        /// <summary>
        /// Called when services have changed on the remote device.
        /// </summary>
        public void OnServiceChanged();

        /// <summary>
        /// Called when a remote RSSI read operation has completed.
        /// </summary>
        /// <param name="status">The status of the RSSI read operation.</param>
        /// <param name="rssi">The RSSI value that was read.</param>
        public void OnReadRemoteRssi(GattStatus status, int rssi);

        /// <summary>
        /// Called when a reliable write operation has completed.
        /// </summary>
        /// <param name="status">The status of the reliable write operation.</param>
        public void OnReliableWriteCompleted(GattStatus status);

        /// <summary>
        /// Called when the MTU (Maximum Transmission Unit) has changed.
        /// </summary>
        /// <param name="status">The status of the MTU change operation.</param>
        /// <param name="mtu">The new MTU value.</param>
        public void OnMtuChanged(GattStatus status, int mtu);

        /// <summary>
        /// Called when PHY read operation has completed.
        /// </summary>
        /// <param name="status">The status of the PHY read operation.</param>
        /// <param name="txPhy">The transmitter PHY in use.</param>
        /// <param name="rxPhy">The receiver PHY in use.</param>
        public void OnPhyRead(GattStatus status, Android.Bluetooth.BluetoothPhy txPhy, Android.Bluetooth.BluetoothPhy rxPhy);

        /// <summary>
        /// Called when PHY update operation has completed.
        /// </summary>
        /// <param name="status">The status of the PHY update operation.</param>
        /// <param name="txPhy">The transmitter PHY in use.</param>
        /// <param name="rxPhy">The receiver PHY in use.</param>
        public void OnPhyUpdate(GattStatus status, Android.Bluetooth.BluetoothPhy txPhy, Android.Bluetooth.BluetoothPhy rxPhy);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
