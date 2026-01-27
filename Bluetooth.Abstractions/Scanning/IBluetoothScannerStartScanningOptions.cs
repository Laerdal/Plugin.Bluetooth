namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Represents a Bluetooth scanner configuration.
/// </summary>
public interface IBluetoothScannerStartScanningOptions
{
    /// <summary>
    /// Gets a value indicating whether duplicate advertisements should be ignored.
    /// </summary>
    bool IgnoreDuplicateAdvertisements { get; }

    /// <summary>
    /// Advertisement filter. If set, only advertisements that pass the filter will be processed.
    /// </summary>
    Func<IBluetoothAdvertisement, bool> AdvertisementFilter { get; }
}
