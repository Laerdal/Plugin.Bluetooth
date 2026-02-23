namespace Bluetooth.Maui.Platforms.Droid.Scanning.Options;

#pragma warning disable CA1008 // Enums should have zero value
/// <summary>
///     Android PHY types for scanning (API 26+).
/// </summary>
/// <remarks>
///     Maps to Android ScanSettings PHY constants.
///     Note: This enum uses [Flags] because Android supports scanning on multiple PHYs simultaneously.
/// </remarks>
[Flags]
public enum ScanPhy
{
    /// <summary>
    ///     All supported PHYs (default).
    ///     Maps to ScanSettings.PHY_LE_ALL_SUPPORTED (value: 0).
    /// </summary>
    AllSupported = 0,

    /// <summary>
    ///     1M PHY only (standard Bluetooth LE).
    ///     Maps to ScanSettings.PHY_LE_1M_MASK (value: 1).
    /// </summary>
    Le1M = 1,

    /// <summary>
    ///     2M PHY (higher throughput, Bluetooth 5.0+).
    ///     Maps to ScanSettings.PHY_LE_2M_MASK (value: 2).
    /// </summary>
    Le2M = 2,

    /// <summary>
    ///     Coded PHY (Long Range, Bluetooth 5.0+).
    ///     Maps to ScanSettings.PHY_LE_CODED_MASK (value: 4).
    /// </summary>
    LeCoded = 4
}

#pragma warning restore CA1008 // Enums should have zero value
