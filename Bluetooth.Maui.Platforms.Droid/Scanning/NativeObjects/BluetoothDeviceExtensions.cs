namespace Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

/// <summary>
/// Provides extension methods for Android BluetoothDevice objects to facilitate GATT connections with various options.
/// </summary>
public static class BluetoothDeviceExtensions
{
    /// <summary>
    /// Connects to the GATT server hosted by this Bluetooth device using the specified connection options and callback.
    /// </summary>
    /// <param name="nativeDevice">The native Bluetooth device to connect to.</param>
    /// <param name="connectionOptions">The options to use when connecting to the GATT server. If null, default options will be used.</param>
    /// <param name="bluetoothGattCallback">The callback to receive GATT events. Cannot be null.</param>
    /// <returns>A BluetoothGatt instance representing the connection to the GATT server, or null if the connection could not be established.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeDevice"/> or <paramref name="bluetoothGattCallback"/> is null.</exception>
    public static BluetoothGatt? ConnectGatt(this Android.Bluetooth.BluetoothDevice nativeDevice, Options.ConnectionOptions? connectionOptions, BluetoothGattCallback? bluetoothGattCallback)
    {
        ArgumentNullException.ThrowIfNull(bluetoothGattCallback);
        ArgumentNullException.ThrowIfNull(nativeDevice);
        connectionOptions ??= new Options.ConnectionOptions();

        if (OperatingSystem.IsAndroidVersionAtLeast(26) && (connectionOptions.PreferredPhy.HasValue))
        {
            // https://developer.android.com/reference/android/bluetooth/BluetoothDevice.html#connectGatt(android.content.Context,%20boolean,%20android.bluetooth.BluetoothGattCallback,%20int,%20int)
            return nativeDevice.ConnectGatt(Android.App.Application.Context,
                                            connectionOptions.AutoConnect,
                                            bluetoothGattCallback,
                                            (Android.Bluetooth.BluetoothTransports)connectionOptions.TransportType,
                                            connectionOptions.PreferredPhy.Value);
        }
        else if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            // https://developer.android.com/reference/android/bluetooth/BluetoothDevice.html#connectGatt(android.content.Context,%20boolean,%20android.bluetooth.BluetoothGattCallback,%20int)
            return nativeDevice.ConnectGatt(Android.App.Application.Context, connectionOptions.AutoConnect, bluetoothGattCallback, (Android.Bluetooth.BluetoothTransports)connectionOptions.TransportType);
        }
        else
        {
            // https://developer.android.com/reference/android/bluetooth/BluetoothDevice.html#connectGatt(android.content.Context,%20boolean,%20android.bluetooth.BluetoothGattCallback)
            return nativeDevice.ConnectGatt(Android.App.Application.Context, connectionOptions.AutoConnect, bluetoothGattCallback);
        }
    }
}
