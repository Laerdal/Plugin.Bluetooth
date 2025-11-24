using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothService : BaseBluetoothService, GattDeviceServiceProxy.IBluetoothServiceProxyDelegate
{
    public GattDeviceServiceProxy NativeServiceProxy { get; }

    public BluetoothService(IBluetoothDevice device, Guid serviceUuid, GattDeviceService nativeService) : base(device, serviceUuid)
    {
        NativeServiceProxy = new GattDeviceServiceProxy(nativeService, this);
    }

    #region BaseBluetoothService

    protected async override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region GattDeviceServiceProxy.IBluetoothServiceProxyDelegate

    public void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus)
    {
        throw new NotImplementedException();
    }

    #endregion

}
