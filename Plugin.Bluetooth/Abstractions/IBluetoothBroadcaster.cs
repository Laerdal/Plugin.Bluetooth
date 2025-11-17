namespace Plugin.Bluetooth.Abstractions;
/// <summary>
/// Interface for managing Bluetooth broadcasting operations, extending <see cref="IBluetoothActivity" />.
/// </summary>
public interface IBluetoothBroadcaster : IBluetoothActivity
{
    /// <summary>
    /// Sets the advertising data asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// - Android: AdvertiseData.Builder.AddServiceUuid
    /// - iOS: CoreBluetooth.StartAdvertisingOptions.ServicesUUID
    /// - Windows: BluetoothLEAdvertisementData.ServiceUuids
    /// </remarks>
    Task NativeSetAdvertisingDataAsync();
}
