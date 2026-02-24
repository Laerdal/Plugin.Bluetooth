namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public record AppleBluetoothLocalCharacteristicSpec : IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothLocalCharacteristicSpec" /> class with the specified Core Bluetooth mutable characteristic.
    /// </summary>
    /// <param name="cbCharacteristic">The native iOS Core Bluetooth mutable characteristic from which to create the factory spec.</param>
    public AppleBluetoothLocalCharacteristicSpec(CBMutableCharacteristic cbCharacteristic) : base(cbCharacteristic?.UUID.ToGuid() ?? throw new ArgumentNullException(nameof(cbCharacteristic)), cbCharacteristic.Properties.ToCharacteristicProperties(),
                                                                                                cbCharacteristic.Permissions.ToCharacteristicPermissions())
    {
        CbCharacteristic = cbCharacteristic;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth mutable characteristic.
    /// </summary>
    public CBMutableCharacteristic CbCharacteristic { get; init; }
}
