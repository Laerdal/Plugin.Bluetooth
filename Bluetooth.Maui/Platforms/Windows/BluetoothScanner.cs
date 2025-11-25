using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

/// <inheritdoc  />
public partial class BluetoothScanner : BaseBluetoothScanner, BluetoothLeAdvertisementWatcherProxy.IBluetoothLeAdvertisementWatcherProxyDelegate, RadioProxy.IRadioProxyDelegate, BluetoothAdapterProxy.IBluetoothAdapterProxyDelegate
{
    public BluetoothLeAdvertisementWatcherProxy? BluetoothLeAdvertisementWatcherProxy { get; protected set; }


    public BluetoothScanner()
    {
    }
}
