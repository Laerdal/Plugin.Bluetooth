namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc/>
public record BluetoothCharacteristicFactoryRequest : IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth characteristic.
    /// </summary>
    public CBCharacteristic? NativeCharacteristic { get; init; }
}
