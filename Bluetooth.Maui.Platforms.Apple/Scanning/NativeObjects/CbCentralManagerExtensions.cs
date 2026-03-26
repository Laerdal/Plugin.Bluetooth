using Bluetooth.Abstractions.Scanning.Options.Apple;

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

        if (scanningOptions is AppleScanningOptions appleScanningOptions)
        {
            if (appleScanningOptions is { ServiceUuid: not null, PeripheralUuids: not null })
            {
                throw new
                    ArgumentException("AppleScanningOptions cannot have both ServiceUuid and PeripheralUuids set. Please provide only one of these options to avoid conflicting filters.");
            }
            else if (appleScanningOptions.ServiceUuid != null)
            {
                // If only ServiceUuid is provided, filter by service UUID
                cbCentralManager.ScanForPeripherals(appleScanningOptions.ServiceUuid.Value.ToCBUuid(),
                                                    new PeripheralScanningOptions
                                                    {
                                                        AllowDuplicatesKey = appleScanningOptions.AllowDuplicates ?? false
                                                    }.Dictionary);
            }
            else if (appleScanningOptions.PeripheralUuids != null)
            {
                // If only PeripheralUuids are provided, filter by peripheral UUIDs
                foreach (var peripheralUuid in appleScanningOptions.PeripheralUuids)
                {
                    cbCentralManager.ScanForPeripherals(peripheralUuid.ToCBUuid(),
                                                        new PeripheralScanningOptions
                                                        {
                                                            AllowDuplicatesKey = appleScanningOptions.AllowDuplicates ?? false
                                                        }.Dictionary);
                }
            }
            else
            {
                // If neither is provided, scan for all peripherals
                cbCentralManager.ScanForPeripherals((CBUUID[]?)null,
                                                    new PeripheralScanningOptions
                                                    {
                                                        AllowDuplicatesKey = appleScanningOptions.AllowDuplicates ?? false
                                                    }.Dictionary);
            }
        }
        else
        {
            // For base ScanningOptions type, scan for all devices (no UUID filtering)
            cbCentralManager.ScanForPeripherals(null, (PeripheralScanningOptions?) null);
        }
    }
}
