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

    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE advertisement publisher is set to anonymous.
    /// </summary>
    bool IsAnonymous { get; }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE advertisement publisher uses extended advertisements.
    /// </summary>
    bool UseExtendedAdvertisement { get; }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE advertisement publisher includes the transmit power level.
    /// </summary>
    bool IncludeTransmitPowerLevel { get; }

    /// <summary>
    /// Gets the preferred transmit power level in dBm for the Bluetooth LE advertisement publisher.
    /// </summary>
    short? PreferredTransmitPowerLevelInDBm { get; }
}
