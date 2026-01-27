namespace Bluetooth.Maui.PlatformSpecific;

public static class BluetoothDeviceExtensions
{
    public static BluetoothGatt? ConnectGatt(this Android.Bluetooth.BluetoothDevice nativeDevice, BluetoothDeviceConnectionOptions? connectionOptions, BluetoothGattCallback? bluetoothGattCallback)
    {
        ArgumentNullException.ThrowIfNull(bluetoothGattCallback);
        ArgumentNullException.ThrowIfNull(nativeDevice);

        if (OperatingSystem.IsAndroidVersionAtLeast(26) && (connectionOptions?.BluetoothPhy.HasValue ?? false))
        {
            // https://developer.android.com/reference/android/bluetooth/BluetoothDevice.html#connectGatt(android.content.Context,%20boolean,%20android.bluetooth.BluetoothGattCallback,%20int,%20int)
            return nativeDevice.ConnectGatt(Android.App.Application.Context,
                                            connectionOptions.UseAutoConnect,
                                            bluetoothGattCallback,
                                            connectionOptions.BluetoothTransports ?? Android.Bluetooth.BluetoothTransports.Le,
                                            connectionOptions.BluetoothPhy.Value);
        }
        else if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            // https://developer.android.com/reference/android/bluetooth/BluetoothDevice.html#connectGatt(android.content.Context,%20boolean,%20android.bluetooth.BluetoothGattCallback,%20int)
            return nativeDevice.ConnectGatt(Android.App.Application.Context, connectionOptions.UseAutoConnect, bluetoothGattCallback, connectionOptions.BluetoothTransports ?? Android.Bluetooth.BluetoothTransports.Le);
        }
        else
        {
            // https://developer.android.com/reference/android/bluetooth/BluetoothDevice.html#connectGatt(android.content.Context,%20boolean,%20android.bluetooth.BluetoothGattCallback)
            return nativeDevice.ConnectGatt(Android.App.Application.Context, connectionOptions.UseAutoConnect, bluetoothGattCallback);
        }
    }
}
