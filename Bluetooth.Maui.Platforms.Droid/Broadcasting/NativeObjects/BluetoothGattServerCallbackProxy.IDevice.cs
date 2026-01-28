namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects
{
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

    public partial class BluetoothGattServerCallbackProxy
    {
        public interface IDevice : IBluetoothBroadcastClientDevice
        {
            void OnMtuChanged(int mtu);

            void OnExecuteWrite(int requestId, bool execute);

            void OnNotificationSent(GattStatus status);

            void OnPhyRead(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy);

            void OnPhyUpdate(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy);

            void OnConnectionStateChange(ProfileState status, ProfileState newState);
        }

    }
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1034 // Nested types should not be visible
}
