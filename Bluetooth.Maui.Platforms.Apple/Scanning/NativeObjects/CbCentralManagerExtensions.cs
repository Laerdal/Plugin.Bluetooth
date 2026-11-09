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

        // The cross-platform list, plus the Apple-specific single UUID. CoreBluetooth reports the peripherals that advertise any of them.
        var serviceUuids = GetServiceUuids(scanningOptions);

        if (scanningOptions is AppleScanningOptions appleScanningOptions)
        {
            if (appleScanningOptions is { ServiceUuid: not null, PeripheralUuids: not null })
            {
                throw new
                    ArgumentException("AppleScanningOptions cannot have both ServiceUuid and PeripheralUuids set. Please provide only one of these options to avoid conflicting filters.");
            }
            else
            {
                // CoreBluetooth cannot scan by peripheral identifier, only by service UUID. With PeripheralUuids set (even empty, as for
                // the ServiceUuid check above) the scan is therefore not filtered natively, and the scanner keeps the listed peripherals
                // when they are discovered. Otherwise filter by the service UUIDs; without any, scan for all peripherals.
                var nativeServiceUuids = appleScanningOptions.PeripheralUuids != null ? null : serviceUuids;
                cbCentralManager.ScanForPeripherals(nativeServiceUuids,
                                                    new PeripheralScanningOptions
                                                    {
                                                        AllowDuplicatesKey = appleScanningOptions.AllowDuplicates ?? false
                                                    }.Dictionary);
            }
        }
        else
        {
            // For base ScanningOptions type, filter by the service UUIDs; without any, scan for all devices
            cbCentralManager.ScanForPeripherals(serviceUuids, (PeripheralScanningOptions?) null);
        }
    }

    /// <summary>
    ///     Gets the service UUIDs to scan for: <see cref="ScanningOptions.ServiceUuids" /> and, for <see cref="AppleScanningOptions" />, its <see cref="AppleScanningOptions.ServiceUuid" />.
    /// </summary>
    /// <returns>The service UUIDs, or <see langword="null" /> when there is none, which scans for all peripherals.</returns>
    private static CBUUID[]? GetServiceUuids(ScanningOptions scanningOptions)
    {
        var serviceUuids = new List<Guid>(scanningOptions.ServiceUuids ?? []);
        if (scanningOptions is AppleScanningOptions { ServiceUuid: { } serviceUuid } && !serviceUuids.Contains(serviceUuid))
        {
            serviceUuids.Add(serviceUuid);
        }

        return serviceUuids.Count == 0 ? null : serviceUuids.Select(uuid => uuid.ToCBUuid()).ToArray();
    }
}
