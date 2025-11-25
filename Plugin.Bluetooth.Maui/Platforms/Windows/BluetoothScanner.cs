using Plugin.Bluetooth.Maui.PlatformSpecific;
using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Plugin.Bluetooth.Maui;

/// <inheritdoc  />
public partial class BluetoothScanner : BaseBluetoothScanner, BluetoothLeAdvertisementWatcherProxy.IBluetoothLeAdvertisementWatcherProxyDelegate, RadioProxy.IRadioProxyDelegate, BluetoothAdapterProxy.IBluetoothAdapterProxyDelegate
{
    public BluetoothLeAdvertisementWatcherProxy? BluetoothLeAdvertisementWatcherProxy { get; protected set; }


    public BluetoothScanner()
    {
    }
}
