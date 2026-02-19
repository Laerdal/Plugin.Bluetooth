using Bluetooth.Maui.Platforms.Droid.Exceptions;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords
public partial class BluetoothGattProxy
{
    /// <summary>
    /// Interface for handling Bluetooth GATT characteristic operations.
    /// Extends the base characteristic interface with Android-specific callback methods.
    /// </summary>
    public interface IBluetoothGattCharacteristicDelegate
    {
        /// <summary>
        /// Called when a characteristic's value has changed.
        /// </summary>
        /// <param name="nativeCharacteristic">The characteristic that changed.</param>
        /// <param name="value">The new value of the characteristic.</param>
        void OnCharacteristicChanged(BluetoothGattCharacteristic? nativeCharacteristic, byte[]? value);

        /// <summary>
        /// Called when a characteristic write operation has completed.
        /// </summary>
        /// <param name="status">The status of the write operation.</param>
        /// <param name="nativeCharacteristic">The characteristic that was written to.</param>
        void OnCharacteristicWrite(GattStatus status, BluetoothGattCharacteristic? nativeCharacteristic);

        /// <summary>
        /// Called when a descriptor read operation has completed.
        /// </summary>
        /// <param name="status">The status of the read operation.</param>
        /// <param name="nativeDescriptor">The descriptor that was read.</param>
        /// <param name="value">The value that was read from the descriptor.</param>
        void OnDescriptorRead(GattStatus status, BluetoothGattDescriptor? nativeDescriptor, byte[]? value);

        /// <summary>
        /// Called when a descriptor write operation has completed.
        /// </summary>
        /// <param name="status">The status of the write operation.</param>
        /// <param name="nativeDescriptor">The descriptor that was written to.</param>
        void OnDescriptorWrite(GattStatus status, BluetoothGattDescriptor? nativeDescriptor);

        /// <summary>
        /// Called when a GATT characteristic read operation completes on the Android platform.
        /// </summary>
        /// <param name="status">The status of the GATT operation.</param>
        /// <param name="nativeCharacteristic">The characteristic that was read.</param>
        /// <param name="value">The value read from the characteristic.</param>
        /// <exception cref="AndroidNativeGattCallbackStatusException">Thrown when the GATT status indicates an error.</exception>
        /// <exception cref="CharacteristicReadException">Thrown when the characteristic UUID doesn't match the expected UUID.</exception>
        void OnCharacteristicRead(GattStatus status, BluetoothGattCharacteristic? nativeCharacteristic, byte[]? value);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords