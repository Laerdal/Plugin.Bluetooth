namespace Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

/// <summary>
///     Interface for the Windows Bluetooth LE Advertisement Watcher wrapper.
///     Provides a testable abstraction over the native Windows BluetoothLEAdvertisementWatcher.
/// </summary>
public interface IBluetoothLeAdvertisementWatcherWrapper
{
    /// <summary>
    ///     Gets the underlying Windows Bluetooth LE Advertisement Watcher.
    ///     Lazily initializes the native watcher on first access.
    /// </summary>
    BluetoothLEAdvertisementWatcher BluetoothLeAdvertisementWatcher { get; }

    /// <summary>
    ///     Gets a value indicating whether extended advertisements are allowed.
    ///     Only available on Windows 10 version 22621 and later.
    /// </summary>
    bool AllowExtendedAdvertisements { get; }

    /// <summary>
    ///     Gets the maximum out-of-range timeout for the watcher.
    /// </summary>
    TimeSpan MaxOutOfRangeTimeout { get; }

    /// <summary>
    ///     Gets the maximum sampling interval for the watcher.
    /// </summary>
    TimeSpan MaxSamplingInterval { get; }

    /// <summary>
    ///     Gets the minimum out-of-range timeout for the watcher.
    /// </summary>
    TimeSpan MinOutOfRangeTimeout { get; }

    /// <summary>
    ///     Gets the minimum sampling interval for the watcher.
    /// </summary>
    TimeSpan MinSamplingInterval { get; }

    /// <summary>
    ///     Gets the signal strength filter in-range threshold in dBm.
    /// </summary>
    short? SignalStrengthFilterInRangeThresholdInDBm { get; }

    /// <summary>
    ///     Gets the signal strength filter out-of-range threshold in dBm.
    /// </summary>
    short? SignalStrengthFilterOutOfRangeThresholdInDBm { get; }

    /// <summary>
    ///     Gets the signal strength filter out-of-range timeout.
    /// </summary>
    TimeSpan? SignalStrengthFilterOutOfRangeTimeout { get; }

    /// <summary>
    ///     Gets the signal strength filter sampling interval.
    /// </summary>
    TimeSpan? SignalStrengthFilterSamplingInterval { get; }

    /// <summary>
    ///     Gets the current status of the Bluetooth LE Advertisement watcher.
    /// </summary>
    BluetoothLEAdvertisementWatcherStatus Status { get; }

    /// <summary>
    ///     Gets the scanning mode of the watcher.
    /// </summary>
    BluetoothLEScanningMode ScanningMode { get; }
}