namespace Bluetooth.Maui.Platforms.Windows.Scanning.NativeObjects;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class BluetoothLeDeviceProxy
{
    /// <summary>
    ///     Delegate interface for handling Bluetooth LE device operations and events.
    ///     Extends the base Bluetooth device interface with Windows-specific device callbacks.
    /// </summary>
    public interface IBluetoothLeDeviceProxyDelegate : IBluetoothRemoteDevice
    {
        /// <summary>
        ///     Called when the GATT services of the device have changed.
        /// </summary>
        void OnGattServicesChanged();

        /// <summary>
        ///     Called when the connection status of the device changes.
        /// </summary>
        /// <param name="newConnectionStatus">The new connection status.</param>
        void OnConnectionStatusChanged(BluetoothConnectionStatus newConnectionStatus);

        /// <summary>
        ///     Called when the device name changes.
        /// </summary>
        /// <param name="senderName">The new device name.</param>
        void OnNameChanged(string senderName);

        /// <summary>
        ///     Called when the device access status changes.
        /// </summary>
        /// <param name="argsId">The device identifier.</param>
        /// <param name="argsStatus">The new access status.</param>
        void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus);

        /// <summary>
        ///     Called when a custom pairing is requested for the device.
        /// </summary>
        /// <param name="args">The pairing request event arguments.</param>
        void OnCustomPairingRequested(DevicePairingRequestedEventArgs args);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix