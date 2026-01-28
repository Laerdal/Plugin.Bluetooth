
namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class BluetoothGattServerCallbackProxy
{
    public interface ICharacteristic : IBluetoothBroadcastCharacteristic
    {
        void OnCharacteristicReadRequest(IDevice sharedDevice, int requestId, int offset);

        void OnCharacteristicWriteRequest(IDevice sharedDevice,
            int requestId,
            bool preparedWrite,
            bool responseNeeded,
            int offset,
            byte[] value);

        void OnDescriptorReadRequest(IDevice sharedDevice, int requestId, int offset, BluetoothGattDescriptor? descriptor);

        void OnDescriptorWriteRequest(IDevice sharedDevice,
            int requestId,
            BluetoothGattDescriptor? descriptor,
            bool preparedWrite,
            bool responseNeeded,
            int offset,
            byte[] value);
    }
}
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1034 // Nested types should not be visible
