namespace Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class BluetoothGattProxy
{
    /// <summary>
    /// Interface for handling Bluetooth GATT service operations.
    /// Extends the base service interface with Android-specific methods.
    /// </summary>
    public interface IBluetoothGattServiceDelegate
    {
        /// <summary>
        /// Gets the characteristic wrapper for the specified GATT characteristic.
        /// </summary>
        /// <param name="nativeCharacteristic">The GATT characteristic to get a wrapper for.</param>
        /// <returns>The characteristic wrapper instance.</returns>
        IBluetoothGattCharacteristicDelegate GetCharacteristic(BluetoothGattCharacteristic? nativeCharacteristic);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords