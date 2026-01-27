using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Represents an iOS-specific Bluetooth Low Energy GATT characteristic.
/// This class wraps iOS Core Bluetooth's CBCharacteristic and provides platform-specific
/// implementations for reading, writing, and listening to characteristic values.
/// </summary>
public partial class BluetoothCharacteristic : BaseBluetoothCharacteristic, CbPeripheralWrapper.ICbCharacteristicDelegate
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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is <c>null</c>.</exception>
    /// <exception cref="CharacteristicFoundInWrongServiceException">Thrown when the characteristic is defined for a different service than the one provided (inherited from base).</exception>
    public BluetoothCharacteristic(IBluetoothService service, Guid id, CBCharacteristic native) : base(service, id)
    {
        NativeCharacteristic = native;
    }
}
