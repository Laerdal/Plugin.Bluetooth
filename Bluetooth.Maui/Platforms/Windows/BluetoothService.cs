using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothService : BaseBluetoothService, GattDeviceServiceProxy.IBluetoothServiceProxyDelegate
{
    public GattDeviceServiceProxy NativeServiceProxy { get; }

    public BluetoothService(IBluetoothDevice device, Guid serviceUuid, GattDeviceService nativeService) : base(device, serviceUuid)
    {
        NativeServiceProxy = new GattDeviceServiceProxy(nativeService, this);
    }

    protected override ValueTask DisposeAsyncCore()
    {
        NativeServiceProxy.Dispose();
        return base.DisposeAsyncCore();
    }

    #region GattDeviceServiceProxy.IBluetoothServiceProxyDelegate

    public void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus)
    {
        // Placeholder for future implementation
    }

    #endregion
}
