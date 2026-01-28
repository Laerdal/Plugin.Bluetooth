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
    public static void ScanForPeripherals(this CBCentralManager cbCentralManager, IBluetoothScannerStartScanningOptions scanningOptions)
    {
        ArgumentNullException.ThrowIfNull(cbCentralManager);
        if (scanningOptions is BluetoothScannerStartScanningOptionsWithServiceUuid scannerStartScanningOptionsWithServiceUuid)
        {
            cbCentralManager.ScanForPeripherals(scannerStartScanningOptionsWithServiceUuid.PeripheralScanningServiceUuid, scannerStartScanningOptionsWithServiceUuid.PeripheralScanningOptions?.Dictionary);
        }
        else if (scanningOptions is BluetoothScannerStartScanningOptionsWithPeripheralUuids scannerStartScanningOptionsWithPeripheralUuids)
        {
            cbCentralManager.ScanForPeripherals(scannerStartScanningOptionsWithPeripheralUuids.PeripheralScanningPeripheralUuids.ToArray(), scannerStartScanningOptionsWithPeripheralUuids.PeripheralScanningOptions);
        }
        else if (scanningOptions is BluetoothScannerStartScanningOptions scannerStartScanningOptions)
        {
            cbCentralManager.ScanForPeripherals(null, scannerStartScanningOptions.PeripheralScanningOptions);
        }
        else
        {
            throw new ArgumentException($"Scanning options must be of type {nameof(BluetoothScannerStartScanningOptionsWithServiceUuid)} or of type {nameof(BluetoothScannerStartScanningOptionsWithPeripheralUuids)} for iOS platform.",
                                        nameof(scanningOptions));
        }
    }

}
