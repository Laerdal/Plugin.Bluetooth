namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc/>
public record BluetoothServiceFactoryRequest : IBluetoothServiceFactory.BluetoothServiceFactoryRequest
{
    /// <summary>
    /// Gets or sets the native Android Bluetooth GATT service.
    /// </summary>
    public Android.Bluetooth.BluetoothGattService? NativeService { get; init; }
}
