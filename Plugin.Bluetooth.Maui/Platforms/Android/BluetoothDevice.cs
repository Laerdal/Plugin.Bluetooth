using Plugin.Bluetooth.Maui.PlatformSpecific;
using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothDevice : BaseBluetoothDevice, BluetoothGattProxy.IDevice, BluetoothDeviceEventProxy.IDevice
{
    public Android.Bluetooth.BluetoothDevice NativeDevice { get; }

    public BluetoothGattProxy? BluetoothGattProxy { get; protected set; }

    public BluetoothDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer, Android.Bluetooth.BluetoothDevice nativeDevice) : base(scanner, id, manufacturer)
    {
        NativeDevice = nativeDevice ?? throw new ArgumentNullException(nameof(nativeDevice));

    }

    public BluetoothDevice(IBluetoothScanner scanner, BluetoothAdvertisement advertisement) : base(scanner, advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);
        ArgumentNullException.ThrowIfNull(advertisement.BluetoothDevice, nameof(advertisement.BluetoothDevice));
        NativeDevice = advertisement.BluetoothDevice;
    }

    public void OnNameChanged(string? newName)
    {
        CachedName = newName ?? CachedName;
    }

    public void OnMtuChanged(GattStatus status, int mtu)
    {
        AndroidNativeGattStatusException.ThrowIfNotSuccess(status);
        // Placeholder for future implementation
    }

    public void OnAclConnected()
    {
        // Placeholder for future implementation
    }

    public void OnClassChanged(BluetoothClass? deviceClass)
    {
        // Placeholder for future implementation
    }

    public void OnUuidChanged(IEnumerable<ParcelUuid>? uuids)
    {
        // Placeholder for future implementation
    }
}
