namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public record AppleBluetoothRemoteCharacteristicFactorySpec : IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteCharacteristicFactorySpec" /> class with the specified Core Bluetooth characteristic.
    /// </summary>
    /// <param name="cbCharacteristic">The native iOS Core Bluetooth characteristic from which to create the factory spec.</param>
    public AppleBluetoothRemoteCharacteristicFactorySpec(CBCharacteristic cbCharacteristic) : base(cbCharacteristic?.UUID.ToGuid() ?? throw new ArgumentNullException(nameof(cbCharacteristic)))
    {
        CbCharacteristic = cbCharacteristic;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth characteristic.
    /// </summary>
    public CBCharacteristic CbCharacteristic { get; init; }
}
