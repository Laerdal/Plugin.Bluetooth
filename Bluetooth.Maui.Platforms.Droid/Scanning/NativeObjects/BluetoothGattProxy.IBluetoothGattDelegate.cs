using BluetoothPhy = Android.Bluetooth.BluetoothPhy;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords
public partial class BluetoothGattProxy
{
    /// <summary>
    ///     Interface for handling Bluetooth GATT device operations.
    ///     Extends the base device interface with Android-specific callback methods.
    /// </summary>
    public interface IBluetoothGattDelegate
    {
        /// <summary>
        ///     Gets the service wrapper for the specified GATT service.
        /// </summary>
        /// <param name="nativeService">The GATT service to get a wrapper for.</param>
        /// <returns>The service wrapper instance.</returns>
        IBluetoothGattServiceDelegate GetService(BluetoothGattService? nativeService);

        /// <summary>
        ///     Called when the connection state changes.
        /// </summary>
        /// <param name="status">The status of the connection state change.</param>
        /// <param name="newState">The new connection state.</param>
        void OnConnectionStateChange(GattStatus status, ProfileState newState);

        /// <summary>
        ///     Called when service discovery has completed.
        /// </summary>
        /// <param name="status">The status of the service discovery operation.</param>
        void OnServicesDiscovered(GattStatus status);

        /// <summary>
        ///     Called when services have changed on the remote device.
        /// </summary>
        void OnServiceChanged();

        /// <summary>
        ///     Called when a remote RSSI read operation has completed.
        /// </summary>
        /// <param name="status">The status of the RSSI read operation.</param>
        /// <param name="rssi">The RSSI value that was read.</param>
        void OnReadRemoteRssi(GattStatus status, int rssi);

        /// <summary>
        ///     Called when a reliable write operation has completed.
        /// </summary>
        /// <param name="status">The status of the reliable write operation.</param>
        void OnReliableWriteCompleted(GattStatus status);

        /// <summary>
        ///     Called when the MTU (Maximum Transmission Unit) has changed.
        /// </summary>
        /// <param name="status">The status of the MTU change operation.</param>
        /// <param name="mtu">The new MTU value.</param>
        void OnMtuChanged(GattStatus status, int mtu);

        /// <summary>
        ///     Called when PHY read operation has completed.
        /// </summary>
        /// <param name="status">The status of the PHY read operation.</param>
        /// <param name="txPhy">The transmitter PHY in use.</param>
        /// <param name="rxPhy">The receiver PHY in use.</param>
        void OnPhyRead(GattStatus status, BluetoothPhy txPhy, BluetoothPhy rxPhy);

        /// <summary>
        ///     Called when PHY update operation has completed.
        /// </summary>
        /// <param name="status">The status of the PHY update operation.</param>
        /// <param name="txPhy">The transmitter PHY in use.</param>
        /// <param name="rxPhy">The receiver PHY in use.</param>
        void OnPhyUpdate(GattStatus status, BluetoothPhy txPhy, BluetoothPhy rxPhy);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords
