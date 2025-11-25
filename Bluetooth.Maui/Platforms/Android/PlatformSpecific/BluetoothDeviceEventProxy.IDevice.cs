namespace Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible

public partial class BluetoothDeviceEventProxy
{
    /// <summary>
    /// Interface for device implementations that handle Android Bluetooth device events.
    /// Extends <see cref="IBluetoothDevice"/> to provide device-specific event handlers.
    /// </summary>
    /// <remarks>
    /// <para>Implementations of this interface receive notifications for various Bluetooth device events
    /// such as bond state changes, pairing requests, and device property changes.</para>
    /// <para>See Android documentation:
    /// <see href="https://developer.android.com/reference/android/bluetooth/BluetoothDevice">BluetoothDevice</see>
    /// </para>
    /// </remarks>
    public interface IDevice : IBluetoothDevice
    {
        /// <summary>
        /// Called when the device's bond state changes.
        /// </summary>
        /// <param name="previousBondState">The previous bond state.</param>
        /// <param name="bondState">The new bond state.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_BOND_STATE_CHANGED. Bond states indicate whether
        /// the device is not bonded, bonding, or bonded (paired).
        /// </remarks>
        void OnBondStateChanged(Bond previousBondState, Bond bondState);

        /// <summary>
        /// Called when a pairing request is received for this device.
        /// </summary>
        /// <param name="pairingVariant">The pairing variant type (e.g., PIN, passkey, consent).</param>
        /// <param name="passKey">The passkey for pairing, if applicable.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_PAIRING_REQUEST. The pairing variant determines
        /// the type of user interaction required to complete pairing.
        /// </remarks>
        void OnPairingRequest(int pairingVariant, int? passKey);

        /// <summary>
        /// Called when an ACL (Asynchronous Connection-Less) connection is established with the device.
        /// </summary>
        /// <remarks>
        /// Corresponds to Android's ACTION_ACL_CONNECTED. ACL connections are low-level connections
        /// managed automatically by the Android Bluetooth stack.
        /// </remarks>
        void OnAclConnected();

        /// <summary>
        /// Called when the device's Bluetooth class changes.
        /// </summary>
        /// <param name="deviceClass">The new device class.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_CLASS_CHANGED. The Bluetooth class describes the
        /// general characteristics and capabilities of the device.
        /// </remarks>
        void OnClassChanged(BluetoothClass? deviceClass);

        /// <summary>
        /// Called when the device's name changes.
        /// </summary>
        /// <param name="newName">The new name of the device.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_NAME_CHANGED. This is called when the friendly name
        /// is retrieved for the first time or changes.
        /// </remarks>
        void OnNameChanged(string? newName);

        /// <summary>
        /// Called when the device's UUIDs are discovered or changed.
        /// </summary>
        /// <param name="uuids">The collection of discovered UUIDs.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_UUID. UUIDs are fetched using Service Discovery Protocol
        /// and identify the services available on the remote device.
        /// </remarks>
        void OnUuidChanged(IEnumerable<ParcelUuid>? uuids);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
