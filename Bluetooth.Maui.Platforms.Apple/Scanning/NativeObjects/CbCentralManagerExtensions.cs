using Bluetooth.Abstractions.Scanning.Options;

using Bluetooth.Maui.Platforms.Apple.Scanning.Options;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

/// <summary>
/// Extension methods for CBCentralManager to support Bluetooth scanning operations.
/// </summary>
public static class CbCentralManagerExtensions
{
    /// <summary>
    /// Starts scanning for peripherals using the specified scanning options.
    /// </summary>
    /// <param name="cbCentralManager">The CBCentralManager instance.</param>
    /// <param name="scanningOptions">The scanning options to use.</param>
    public static void ScanForPeripherals(this CBCentralManager cbCentralManager, Abstractions.Scanning.Options.ScanningOptions scanningOptions)
    {
        ArgumentNullException.ThrowIfNull(cbCentralManager);
        if (scanningOptions is ScanningOptionsWithServiceUuid scannerStartScanningOptionsWithServiceUuid)
        {
            cbCentralManager.ScanForPeripherals(scannerStartScanningOptionsWithServiceUuid.PeripheralScanningServiceUuid, scannerStartScanningOptionsWithServiceUuid.PeripheralScanningOptions?.Dictionary);
        }
        else if (scanningOptions is ScanningOptionsWithPeripheralUuids scannerStartScanningOptionsWithPeripheralUuids)
        {
            cbCentralManager.ScanForPeripherals(scannerStartScanningOptionsWithPeripheralUuids.PeripheralScanningPeripheralUuids.ToArray(), scannerStartScanningOptionsWithPeripheralUuids.PeripheralScanningOptions);
        }
        else if (scanningOptions is Bluetooth.Maui.Platforms.Apple.Scanning.Options.ScanningOptions scannerStartScanningOptions)
        {
            cbCentralManager.ScanForPeripherals(null, scannerStartScanningOptions.PeripheralScanningOptions);
        }
        else
        {
            throw new ArgumentException($"Scanning options must be of type {nameof(ScanningOptionsWithServiceUuid)} or of type {nameof(ScanningOptionsWithPeripheralUuids)} for iOS platform.",
                                        nameof(scanningOptions));
        }
    }

}
