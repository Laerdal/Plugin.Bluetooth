namespace Bluetooth.Maui.Platforms.Apple.Scanning.Options;

/// <summary>
///     iOS-specific Bluetooth scanner start scanning options.
/// </summary>
public record ScanningOptions : Abstractions.Scanning.Options.ScanningOptions
{
    /// <summary>
    ///     Gets or sets the peripheral scanning options.
    /// </summary>
    /// <remarks>
    ///     These options control whether to allow duplicate advertisements and other scanning behaviors.
    /// </remarks>
    public PeripheralScanningOptions? PeripheralScanningOptions { get; set; }
}
