using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <inheritdoc/>
public partial class BluetoothCharacteristic : BaseBluetoothCharacteristic, CbPeripheralProxy.ICbCharacteristicDelegate
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth characteristic used for iOS Bluetooth operations.
    /// </summary>
    public CBCharacteristic NativeCharacteristic { get; }

    /// <summary>
    /// Initializes a new instance of the iOS <see cref="BluetoothCharacteristic"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with this characteristic.</param>
    /// <param name="id">The unique identifier of the characteristic.</param>
    /// <param name="native">The native iOS Core Bluetooth characteristic.</param>
    public BluetoothCharacteristic(IBluetoothService service, Guid id, CBCharacteristic native) : base(service, id)
    {
        NativeCharacteristic = native;
    }
}
