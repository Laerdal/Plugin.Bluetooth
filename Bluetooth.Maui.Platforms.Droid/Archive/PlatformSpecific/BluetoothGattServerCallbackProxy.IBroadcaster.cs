namespace Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class BluetoothGattServerCallbackProxy
{
    public interface IBroadcaster : IBluetoothBroadcaster
    {
        public BluetoothGattServerCallbackProxy.IDevice GetDevice(Android.Bluetooth.BluetoothDevice? native);

        public BluetoothGattServerCallbackProxy.IService GetService(Android.Bluetooth.BluetoothGattService? native);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
