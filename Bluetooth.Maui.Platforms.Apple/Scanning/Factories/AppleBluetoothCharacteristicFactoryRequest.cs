using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public record AppleBluetoothCharacteristicFactoryRequest : IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothCharacteristicFactoryRequest" /> class with the specified Core Bluetooth characteristic.
    /// </summary>
    /// <param name="cbCharacteristic">The native iOS Core Bluetooth characteristic from which to create the factory request.</param>
    public AppleBluetoothCharacteristicFactoryRequest([NotNull] CBCharacteristic cbCharacteristic) : base(cbCharacteristic.UUID.ToGuid())
    {
        CbCharacteristic = cbCharacteristic;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth characteristic.
    /// </summary>
    public CBCharacteristic CbCharacteristic { get; init; }
}