namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class BluetoothGattServerCallbackProxy
{
    /// <summary>
    /// Delegate interface for handling GATT service-level events.
    /// </summary>
    public interface IBluetoothGattServiceDelegate
    {
        /// <summary>
        /// Gets the characteristic delegate for the specified native GATT characteristic.
        /// </summary>
        /// <param name="native">The native Android GATT characteristic.</param>
        /// <returns>The characteristic delegate for the specified characteristic.</returns>
        BluetoothGattServerCallbackProxy.IBluetoothGattCharacteristicDelegate GetCharacteristic(Android.Bluetooth.BluetoothGattCharacteristic? native);

        /// <summary>
        /// Called when a service has been added to the GATT server.
        /// </summary>
        /// <param name="status">The status of the service addition operation.</param>
        void OnServiceAdded(GattStatus status);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords
