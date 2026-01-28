namespace Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

#pragma warning disable CA1034 // Nested types should not be visible

public partial class BluetoothGattProxy
{
    /// <summary>
    /// Interface for handling Bluetooth GATT service operations.
    /// Extends the base service interface with Android-specific methods.
    /// </summary>
    public interface IService : IBluetoothService
    {
        /// <summary>
        /// Gets the characteristic wrapper for the specified GATT characteristic.
        /// </summary>
        /// <param name="nativeCharacteristic">The GATT characteristic to get a wrapper for.</param>
        /// <returns>The characteristic wrapper instance.</returns>
        ICharacteristic GetCharacteristic(BluetoothGattCharacteristic? nativeCharacteristic);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
