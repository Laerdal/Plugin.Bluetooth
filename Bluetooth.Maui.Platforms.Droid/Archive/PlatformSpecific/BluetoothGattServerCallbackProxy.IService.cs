namespace Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class BluetoothGattServerCallbackProxy
{
    public interface IService : IBluetoothBroadcastService
    {
        BluetoothGattServerCallbackProxy.ICharacteristic GetCharacteristic(Android.Bluetooth.BluetoothGattCharacteristic? native);

        void OnServiceAdded(GattStatus status);
    }
}
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1034 // Nested types should not be visible
