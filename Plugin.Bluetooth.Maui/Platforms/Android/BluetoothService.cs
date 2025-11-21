using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothService : BaseBluetoothService, BluetoothGattProxy.IService
{
    public BluetoothGattService NativeService { get; }

    public BluetoothService(IBluetoothDevice device, Guid serviceUuid, BluetoothGattService nativeService) : base(device, serviceUuid)
    {
        NativeService = nativeService;
    }

}
