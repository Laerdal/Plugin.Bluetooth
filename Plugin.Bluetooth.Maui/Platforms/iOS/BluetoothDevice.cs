using Plugin.Bluetooth.Maui.PlatformSpecific;
using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothDevice : BaseBluetoothDevice, CbPeripheralProxy.ICbPeripheralProxyDelegate, CbCentralManagerProxy.ICbPeripheralDelegate
{
    public CbPeripheralProxy CbPeripheralDelegateProxy { get; }

    public BluetoothDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer, CbPeripheralProxy cbPeripheralDelegateProxy) : base(scanner, id, manufacturer)
    {
        CbPeripheralDelegateProxy = cbPeripheralDelegateProxy;
    }

    public BluetoothDevice(IBluetoothScanner scanner, BluetoothAdvertisement advertisement) : base(scanner, advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);
        CbPeripheralDelegateProxy = new CbPeripheralProxy(this, advertisement.Peripheral);
    }

    #region CbPeripheralProxy.ICbPeripheralProxyDelegate

    /// <inheritdoc/>
    public void UpdatedName()
    {
        if (CbPeripheralDelegateProxy.CbPeripheral.Name != null)
        {
            CachedName = CbPeripheralDelegateProxy.CbPeripheral.Name;
        }
    }

    /// <inheritdoc/>
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        AppleNativeBluetoothException.ThrowIfError(error);
        // Placeholder for future implementation
    }

    #endregion
}
