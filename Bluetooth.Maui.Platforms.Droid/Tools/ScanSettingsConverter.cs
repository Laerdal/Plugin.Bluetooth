using Bluetooth.Maui.Platforms.Droid.Scanning.Options;

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
}
