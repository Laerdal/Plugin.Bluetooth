namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class BluetoothGattServerCallbackProxy
{
    /// <summary>
    ///     Delegate interface for handling GATT characteristic-level events.
    /// </summary>
    public interface IBluetoothGattCharacteristicDelegate
    {
        /// <summary>
        ///     Called when a remote device has requested to read a characteristic.
        /// </summary>
        /// <param name="sharedBluetoothDeviceDelegate">The device delegate for the requesting device.</param>
        /// <param name="requestId">The ID of the read spec.</param>
        /// <param name="offset">The offset from which to read the characteristic value.</param>
        void OnCharacteristicReadRequest(IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate, int requestId, int offset);

        /// <summary>
        ///     Called when a remote device has requested to write to a characteristic.
        /// </summary>
        /// <param name="sharedBluetoothDeviceDelegate">The device delegate for the requesting device.</param>
        /// <param name="requestId">The ID of the write spec.</param>
        /// <param name="preparedWrite">Whether this is a prepared write operation.</param>
        /// <param name="responseNeeded">Whether the remote device requires a response.</param>
        /// <param name="offset">The offset at which to write the characteristic value.</param>
        /// <param name="value">The value to be written to the characteristic.</param>
        void OnCharacteristicWriteRequest(IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate,
            int requestId,
            bool preparedWrite,
            bool responseNeeded,
            int offset,
            byte[] value);

        /// <summary>
        ///     Gets the descriptor delegate for the specified native descriptor.
        /// </summary>
        /// <param name="native">The native Bluetooth GATT descriptor.</param>
        /// <returns>The descriptor delegate for the specified descriptor.</returns>
        IBluetoothGattDescriptorDelegate GetDescriptor(BluetoothGattDescriptor? native);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords
