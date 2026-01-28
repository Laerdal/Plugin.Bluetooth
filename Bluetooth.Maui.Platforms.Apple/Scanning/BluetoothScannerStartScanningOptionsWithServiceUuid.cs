namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
/// Base class for iOS-specific Bluetooth scanner start scanning options.
/// </summary>
public record BluetoothScannerStartScanningOptionsWithServiceUuid : BluetoothScannerStartScanningOptions
{
    /// <summary>
    /// Gets or sets the service UUID to filter peripherals during scanning.
    /// </summary>
    /// <remarks>
    /// If <c>null</c>, all peripherals will be discovered. If specified, only peripherals advertising this service will be reported.
    /// </remarks>
    public required CBUUID PeripheralScanningServiceUuid { get; init; }
}
