using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public record AndroidBluetoothRemoteCharacteristicFactorySpec : IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteCharacteristicFactorySpec" /> class with the specified Android GATT characteristic.
    /// </summary>
    /// <param name="nativeCharacteristic">The native Android GATT characteristic from which to create the factory spec.</param>
    public AndroidBluetoothRemoteCharacteristicFactorySpec(BluetoothGattCharacteristic nativeCharacteristic)
        : base(nativeCharacteristic?.Uuid?.ToGuid() ?? throw new ArgumentNullException(nameof(nativeCharacteristic)))
    {
        NativeCharacteristic = nativeCharacteristic;
    }

    /// <summary>
    ///     Gets the native Android GATT characteristic.
    /// </summary>
    public BluetoothGattCharacteristic NativeCharacteristic { get; init; }
}
