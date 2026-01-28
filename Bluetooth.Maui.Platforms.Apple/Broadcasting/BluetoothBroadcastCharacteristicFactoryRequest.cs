namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc/>
public record BluetoothBroadcastCharacteristicFactoryRequest : IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest
{
    /// <summary>
    /// Gets the native iOS mutable characteristic.
    /// </summary>
    public CBMutableCharacteristic? NativeCharacteristic { get; init; }
}
