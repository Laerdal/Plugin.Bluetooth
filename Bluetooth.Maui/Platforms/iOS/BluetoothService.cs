using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothService : BaseBluetoothService, CbPeripheralProxy.ICbServiceDelegate
{
    public CBService NativeService { get; }

    public BluetoothService(IBluetoothDevice device, Guid serviceUuid, CBService nativeService) : base(device, serviceUuid)
    {
        NativeService = nativeService;
    }

    #region CbPeripheralProxy.ICbServiceDelegate
    public void DiscoveredIncludedService(NSError? error, CBService service)
    {
        AppleNativeBluetoothException.ThrowIfError(error);
    }

    #endregion
}
