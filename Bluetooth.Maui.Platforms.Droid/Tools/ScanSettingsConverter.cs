using Bluetooth.Abstractions.Scanning.Options.Android;

namespace Bluetooth.Maui.Platforms.Droid.Tools;

/// <summary>
///     Provides extension methods for converting Android-specific scan settings options to native Android types.
/// </summary>
public static class ScanSettingsConverter
{
    /// <summary>
    ///     Converts a <see cref="ScanMatchMode" /> value to the corresponding Android <see cref="BluetoothScanMatchMode" /> value.
    /// </summary>
    /// <remarks>
    ///     Controls how aggressively the Bluetooth stack filters scan results.
    ///     Requires Android 6.0 (API 23) or higher.
    /// </remarks>
    public static BluetoothScanMatchMode ToAndroidMatchMode(this ScanMatchMode matchMode)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            throw new PlatformNotSupportedException("Match mode requires Android 6.0 (API 23) or higher");
        }

        return matchMode switch
        {
            ScanMatchMode.Aggressive => (BluetoothScanMatchMode) 1, // MATCH_MODE_AGGRESSIVE
            ScanMatchMode.Sticky => (BluetoothScanMatchMode) 2,     // MATCH_MODE_STICKY
            _ => (BluetoothScanMatchMode) 1
        };
    }

    /// <summary>
    ///     Converts a <see cref="ScanMatchNumber" /> value to the corresponding Android number of matches value.
    /// </summary>
    /// <remarks>
    ///     Controls when scan results are reported based on the number of matched advertisements.
    ///     Requires Android 6.0 (API 23) or higher.
    /// </remarks>
    public static BluetoothScanMatchNumber ToAndroidNumOfMatches(this ScanMatchNumber numOfMatches)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            throw new PlatformNotSupportedException("Match count requires Android 6.0 (API 23) or higher");
        }

        return numOfMatches switch
        {
            ScanMatchNumber.One => BluetoothScanMatchNumber.OneAdvertisement,   // MATCH_NUM_ONE_ADVERTISEMENT
            ScanMatchNumber.Few => BluetoothScanMatchNumber.FewAdvertisement,  // MATCH_NUM_FEW_ADVERTISEMENT
            ScanMatchNumber.Max => BluetoothScanMatchNumber.MaxAdvertisement,  // MATCH_NUM_MAX_ADVERTISEMENT
            _ => BluetoothScanMatchNumber.OneAdvertisement
        };
    }

    /// <summary>
    ///     Converts a <see cref="ScanPhy" /> value to the corresponding Android <see cref="Android.Bluetooth.BluetoothPhy" /> value.
    /// </summary>
    /// <remarks>
    ///     Maps abstract scan PHY preferences to native Android PHY types.
    ///     Requires Android 8.0 (API 26) or higher.
    ///     Note: AllSupported and flag combinations fall back to PHY_LE_ALL_SUPPORTED (255).
    /// </remarks>
    public static Android.Bluetooth.BluetoothPhy ToAndroidScanPhy(this ScanPhy phy)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            throw new PlatformNotSupportedException("PHY selection for scanning requires Android 8.0 (API 26) or higher");
        }

        return phy switch
        {
            ScanPhy.Le1M => Android.Bluetooth.BluetoothPhy.Le1m,
            ScanPhy.Le2M => Android.Bluetooth.BluetoothPhy.Le2m,
            ScanPhy.LeCoded => (Android.Bluetooth.BluetoothPhy) 4, // PHY_LE_CODED_MASK bitmask value
            _ => (Android.Bluetooth.BluetoothPhy) 255             // PHY_LE_ALL_SUPPORTED
        };
    }
}
