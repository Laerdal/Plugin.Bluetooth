namespace Bluetooth.Maui.Platforms.Apple.Scanning.Options;

/// <summary>
///     iOS-specific Bluetooth scanner start scanning options that include filtering by multiple peripheral UUIDs.
/// </summary>
public record ScanningOptionsWithPeripheralUuids : ScanningOptions
{
    /// <summary>
    ///     Gets or sets the list of service UUIDs to filter peripherals during scanning.
    /// </summary>
    /// <remarks>
    ///     If empty, all peripherals will be discovered. If specified, only peripherals advertising these services will be reported.
    /// </remarks>
    public IReadOnlyList<CBUUID> PeripheralScanningPeripheralUuids { get; set; } = [];
}