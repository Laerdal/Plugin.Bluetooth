namespace Bluetooth.Maui;

/// <summary>
/// iOS-specific Bluetooth scanner start scanning options.
/// </summary>
public record BluetoothScannerStartScanningOptions : BaseBluetoothScannerStartScanningOptions
{
    /// <summary>
    /// Gets or sets the peripheral scanning options.
    /// </summary>
    /// <remarks>
    /// These options control whether to allow duplicate advertisements and other scanning behaviors.
    /// </remarks>
    public PeripheralScanningOptions? PeripheralScanningOptions { get; init; }
}
