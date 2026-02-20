namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class BluetoothGattServerCallbackProxy
{
    /// <summary>
    ///     Delegate interface for the GATT server callback proxy, providing access to device and service delegates.
    /// </summary>
    public interface IBluetoothGattServerCallbackProxyDelegate
    {
        /// <summary>
        ///     Gets the device delegate for the specified native Bluetooth device.
        /// </summary>
        /// <param name="native">The native Android Bluetooth device.</param>
        /// <returns>The device delegate for the specified device.</returns>
        IBluetoothDeviceDelegate GetDevice(BluetoothDevice? native);

        /// <summary>
        ///     Gets the service delegate for the specified native GATT service.
        /// </summary>
        /// <param name="native">The native Android GATT service.</param>
        /// <returns>The service delegate for the specified service.</returns>
        IBluetoothGattServiceDelegate GetService(BluetoothGattService? native);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords