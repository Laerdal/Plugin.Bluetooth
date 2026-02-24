namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class BluetoothGattServerCallbackProxy
{
    /// <summary>
    ///     Delegate interface for handling Bluetooth device-level events.
    /// </summary>
    public interface IBluetoothDeviceDelegate
    {
        /// <summary>
        ///     Called when the MTU (Maximum Transmission Unit) for a device has changed.
        /// </summary>
        /// <param name="mtu">The new MTU size in bytes.</param>
        void OnMtuChanged(int mtu);

        /// <summary>
        ///     Called when a remote device has requested to execute a prepared write.
        /// </summary>
        /// <param name="requestId">The ID of the spec.</param>
        /// <param name="execute">Whether to execute (true) or cancel (false) the prepared writes.</param>
        void OnExecuteWrite(int requestId, bool execute);

        /// <summary>
        ///     Called when a notification or indication has been sent to a remote device.
        /// </summary>
        /// <param name="status">The status of the notification operation.</param>
        void OnNotificationSent(GattStatus status);

        /// <summary>
        ///     Called when the PHY (Physical Layer) read operation completes.
        /// </summary>
        /// <param name="status">The status of the PHY read operation.</param>
        /// <param name="txPhy">The transmitter PHY in use.</param>
        /// <param name="rxPhy">The receiver PHY in use.</param>
        void OnPhyRead(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy);

        /// <summary>
        ///     Called when the PHY (Physical Layer) has been updated.
        /// </summary>
        /// <param name="status">The status of the PHY update operation.</param>
        /// <param name="txPhy">The new transmitter PHY.</param>
        /// <param name="rxPhy">The new receiver PHY.</param>
        void OnPhyUpdate(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy);

        /// <summary>
        ///     Called when the connection state of a remote device has changed.
        /// </summary>
        /// <param name="status">The current connection status.</param>
        /// <param name="newState">The new connection state.</param>
        void OnConnectionStateChange(ProfileState status, ProfileState newState);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords
