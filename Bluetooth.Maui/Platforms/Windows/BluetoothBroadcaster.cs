using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Windows implementation of the Bluetooth broadcaster using Windows.Devices.Bluetooth.Advertisement APIs.
/// </summary>
/// <remarks>
/// This implementation allows the device to broadcast BLE advertisements.
/// Most functionality is not yet implemented.
/// </remarks>
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, BluetoothLeAdvertisementPublisherProxy.IBluetoothLeAdvertisementPublisherProxyDelegate, RadioProxy.IRadioProxyDelegate, BluetoothAdapterProxy.IBluetoothAdapterProxyDelegate
{
    /// <summary>
    /// Gets the Bluetooth LE advertisement publisher proxy.
    /// </summary>
    public BluetoothLeAdvertisementPublisherProxy? BluetoothLeAdvertisementPublisherProxy { get; protected set; }


}
