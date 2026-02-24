using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <summary>
///     Windows-specific implementation of the Bluetooth characteristic factory spec.
/// </summary>
public record WindowsBluetoothRemoteCharacteristicFactorySpec : IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothRemoteCharacteristicFactorySpec" /> record.
    /// </summary>
    /// <param name="nativeCharacteristic">The native Windows GATT characteristic.</param>
    public WindowsBluetoothRemoteCharacteristicFactorySpec(GattCharacteristic nativeCharacteristic)
        : base(nativeCharacteristic?.Uuid ?? throw new ArgumentNullException(nameof(nativeCharacteristic)))
    {
        ArgumentNullException.ThrowIfNull(nativeCharacteristic);
        NativeCharacteristic = nativeCharacteristic;
    }

    /// <summary>
    ///     Gets the native Windows GATT characteristic.
    /// </summary>
    public GattCharacteristic NativeCharacteristic { get; }
}
