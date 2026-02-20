using Bluetooth.Maui.Platforms.Apple.Scanning.Options;

using ScanningOptions = Bluetooth.Abstractions.Scanning.Options.ScanningOptions;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

/// <summary>
///     Extension methods for CBCentralManager to support Bluetooth scanning operations.
/// </summary>
public static class CbCentralManagerExtensions
{
    /// <summary>
    ///     Starts scanning for peripherals using the specified scanning options.
    /// </summary>
    /// <param name="cbCentralManager">The CBCentralManager instance.</param>
    /// <param name="scanningOptions">The scanning options to use.</param>
    public static void ScanForPeripherals(this CBCentralManager cbCentralManager, ScanningOptions scanningOptions)
    {
        ArgumentNullException.ThrowIfNull(cbCentralManager);
        ArgumentNullException.ThrowIfNull(scanningOptions);

        if (scanningOptions is ScanningOptionsWithServiceUuid scannerStartScanningOptionsWithServiceUuid)
        {
            cbCentralManager.ScanForPeripherals(scannerStartScanningOptionsWithServiceUuid.PeripheralScanningServiceUuid, scannerStartScanningOptionsWithServiceUuid.PeripheralScanningOptions?.Dictionary);
        }
        else if (scanningOptions is ScanningOptionsWithPeripheralUuids scannerStartScanningOptionsWithPeripheralUuids)
        {
            cbCentralManager.ScanForPeripherals(scannerStartScanningOptionsWithPeripheralUuids.PeripheralScanningPeripheralUuids.ToArray(), scannerStartScanningOptionsWithPeripheralUuids.PeripheralScanningOptions);
        }
        else if (scanningOptions is Options.ScanningOptions scannerStartScanningOptions)
        {
            cbCentralManager.ScanForPeripherals(null, scannerStartScanningOptions.PeripheralScanningOptions);
        }
        else
        {
            // For base ScanningOptions type, scan for all devices (no UUID filtering)
            cbCentralManager.ScanForPeripherals(null, (PeripheralScanningOptions?) null);
        }
    }
}