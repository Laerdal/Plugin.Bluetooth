using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

/// <summary>
/// Windows implementation of the Bluetooth scanner using Windows.Devices.Bluetooth APIs.
/// </summary>
/// <remarks>
/// This implementation uses <see cref="BluetoothLEAdvertisementWatcher"/> to monitor BLE advertisements.
/// </remarks>
public partial class BluetoothScanner : BaseBluetoothScanner, BluetoothLeAdvertisementWatcherProxy.IBluetoothLeAdvertisementWatcherProxyDelegate, RadioProxy.IRadioProxyDelegate, BluetoothAdapterProxy.IBluetoothAdapterProxyDelegate
{
    /// <summary>
    /// Gets the Bluetooth LE advertisement watcher proxy.
    /// </summary>
    public BluetoothLeAdvertisementWatcherProxy? BluetoothLeAdvertisementWatcherProxy { get; protected set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothScanner"/> class.
    /// </summary>
    public BluetoothScanner()
    {
    }
}
