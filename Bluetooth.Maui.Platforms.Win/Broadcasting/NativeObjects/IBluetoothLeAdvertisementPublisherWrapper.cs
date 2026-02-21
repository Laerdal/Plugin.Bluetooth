namespace Bluetooth.Maui.Platforms.Win.Broadcasting.NativeObjects;

/// <summary>
///     Interface for the Windows Bluetooth LE Advertisement Publisher wrapper.
///     Provides a testable abstraction over the native Windows BluetoothLEAdvertisementPublisher.
/// </summary>
public interface IBluetoothLeAdvertisementPublisherWrapper
{
    /// <summary>
    ///     Gets the underlying Windows Bluetooth LE Advertisement Publisher.
    ///     Lazily initializes the native publisher on first access.
    /// </summary>
    BluetoothLEAdvertisementPublisher BluetoothLeAdvertisementPublisher { get; }

    /// <summary>
    ///     Gets the current status of the Bluetooth LE Advertisement publisher.
    /// </summary>
    BluetoothLEAdvertisementPublisherStatus Status { get; }
}
