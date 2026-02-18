using System.Diagnostics.CodeAnalysis;

using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <summary>
/// Windows-specific implementation of the Bluetooth characteristic factory request.
/// </summary>
public record BluetoothCharacteristicFactoryRequest : IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest
{
    /// <summary>
    /// Gets the native Windows GATT characteristic.
    /// </summary>
    public GattCharacteristic NativeCharacteristic { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothCharacteristicFactoryRequest"/> record.
    /// </summary>
    /// <param name="nativeCharacteristic">The native Windows GATT characteristic.</param>
    public BluetoothCharacteristicFactoryRequest([NotNull]GattCharacteristic nativeCharacteristic)
        : base(nativeCharacteristic.Uuid)
    {
        ArgumentNullException.ThrowIfNull(nativeCharacteristic);
        NativeCharacteristic = nativeCharacteristic;
    }
}
