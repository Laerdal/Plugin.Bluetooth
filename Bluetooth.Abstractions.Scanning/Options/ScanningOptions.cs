using Bluetooth.Abstractions.Scanning.Options.Android;

namespace Bluetooth.Abstractions.Scanning.Options;

public record ScanningOptions
{
    /// <summary>
    ///     Advertisement filter. If set, only advertisements that pass the filter will be processed.
    /// </summary>
    public Func<IBluetoothAdvertisement, bool> AdvertisementFilter { get; init; } = _ => true;
}
