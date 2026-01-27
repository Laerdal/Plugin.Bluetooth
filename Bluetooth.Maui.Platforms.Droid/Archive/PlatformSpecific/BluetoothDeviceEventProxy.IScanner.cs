namespace Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible

public partial class BluetoothDeviceEventProxy
{
    /// <summary>
    /// Interface for scanner implementations that handle device discovery and event routing.
    /// Extends <see cref="IBluetoothScanner"/> to provide scanner-specific methods.
    /// </summary>
    /// <remarks>
    /// <para>Implementations are responsible for managing discovered devices and routing device events
    /// to the appropriate device instances.</para>
    /// <para>See Android documentation:
    /// <see href="https://developer.android.com/reference/android/bluetooth/BluetoothAdapter#startDiscovery()">BluetoothAdapter.startDiscovery()</see>
    /// </para>
    /// </remarks>
    public interface IScanner : IBluetoothScanner
    {
        /// <summary>
        /// Retrieves a device instance for the specified native Android Bluetooth device.
        /// </summary>
        /// <param name="nativeDevice">The native Android Bluetooth device.</param>
        /// <returns>The device instance corresponding to the native device.</returns>
        /// <remarks>
        /// This method is used to map native Android BluetoothDevice objects to the plugin's
        /// device wrapper instances for event handling.
        /// </remarks>
        public IDevice GetDevice(Android.Bluetooth.BluetoothDevice? nativeDevice);

        /// <summary>
        /// Called when a Bluetooth device is discovered during scanning.
        /// </summary>
        /// <param name="device">The discovered Bluetooth device.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_FOUND broadcast action. This is called when a remote device
        /// is found during discovery initiated by BluetoothAdapter.startDiscovery().
        /// </remarks>
        void OnDeviceFound(Android.Bluetooth.BluetoothDevice device);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
