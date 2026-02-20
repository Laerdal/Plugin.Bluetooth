using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public record AppleBluetoothCharacteristicSpec : IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothCharacteristicSpec" /> class with the specified Core Bluetooth mutable characteristic.
    /// </summary>
    /// <param name="cbCharacteristic">The native iOS Core Bluetooth mutable characteristic from which to create the factory request.</param>
    public AppleBluetoothCharacteristicSpec([NotNull] CBMutableCharacteristic cbCharacteristic) : base(cbCharacteristic.UUID.ToGuid(), cbCharacteristic.Properties.ToCharacteristicProperties(),
        cbCharacteristic.Permissions.ToCharacteristicPermissions())
    {
        CbCharacteristic = cbCharacteristic;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth mutable characteristic.
    /// </summary>
    public CBMutableCharacteristic CbCharacteristic { get; init; }
}