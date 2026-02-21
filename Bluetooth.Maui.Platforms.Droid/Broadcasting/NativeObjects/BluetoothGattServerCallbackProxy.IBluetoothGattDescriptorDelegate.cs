namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class BluetoothGattServerCallbackProxy
{
    /// <summary>
    ///     Delegate interface for handling GATT descriptor-level events.
    /// </summary>
    public interface IBluetoothGattDescriptorDelegate
    {
        /// <summary>
        ///     Called when a remote device has requested to read a descriptor.
        /// </summary>
        /// <param name="sharedBluetoothDeviceDelegate">The device delegate for the requesting device.</param>
        /// <param name="requestId">The ID of the read request.</param>
        /// <param name="offset">The offset from which to read the descriptor value.</param>
        /// <param name="descriptor">The descriptor being read.</param>
        void OnDescriptorReadRequest(IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate, int requestId, int offset, BluetoothGattDescriptor? descriptor);

        /// <summary>
        ///     Called when a remote device has requested to write to a descriptor.
        /// </summary>
        /// <param name="sharedBluetoothDeviceDelegate">The device delegate for the requesting device.</param>
        /// <param name="requestId">The ID of the write request.</param>
        /// <param name="descriptor">The descriptor being written to.</param>
        /// <param name="preparedWrite">Whether this is a prepared write operation.</param>
        /// <param name="responseNeeded">Whether the remote device requires a response.</param>
        /// <param name="offset">The offset at which to write the descriptor value.</param>
        /// <param name="value">The value to be written to the descriptor.</param>
        void OnDescriptorWriteRequest(IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate,
            int requestId,
            BluetoothGattDescriptor? descriptor,
            bool preparedWrite,
            bool responseNeeded,
            int offset,
            byte[] value);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords