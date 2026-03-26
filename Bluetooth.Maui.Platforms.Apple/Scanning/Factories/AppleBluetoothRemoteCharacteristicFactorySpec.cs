using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple-specific factory spec for creating <see cref="AppleBluetoothRemoteCharacteristic" /> instances.
///     Extends the base spec with the native <see cref="CBCharacteristic" /> required for CoreBluetooth.
/// </summary>
public record AppleBluetoothRemoteCharacteristicFactorySpec(Guid CharacteristicId, CBCharacteristic CbCharacteristic)
    : IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec(CharacteristicId)
{
    /// <summary>
    ///     Initializes a new instance from a native Core Bluetooth characteristic.
    /// </summary>
    /// <param name="cbCharacteristic">The native iOS Core Bluetooth characteristic.</param>
    public AppleBluetoothRemoteCharacteristicFactorySpec(CBCharacteristic cbCharacteristic)
        : this((cbCharacteristic ?? throw new ArgumentNullException(nameof(cbCharacteristic))).UUID.ToGuid(), cbCharacteristic)
    {
    }
}
