namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc/>
public record BluetoothCharacteristicFactoryRequest : IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest
{
    /// <summary>
    /// Gets or sets the native Android Bluetooth characteristic.
    /// </summary>
    public Android.Bluetooth.BluetoothGattCharacteristic? NativeCharacteristic { get; init; }
}
