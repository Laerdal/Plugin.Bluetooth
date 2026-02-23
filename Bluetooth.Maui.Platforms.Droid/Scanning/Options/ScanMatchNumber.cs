namespace Bluetooth.Maui.Platforms.Droid.Scanning.Options;

#pragma warning disable CA1008 // Enums should have zero value

/// <summary>
///     Android scan match numbers (API 23+).
/// </summary>
/// <remarks>
///    Maps to Android ScanSettings match number constants.
/// </remarks>
public enum ScanMatchNumber
{
    /// <summary>
    ///     Report after matching one advertisement.
    ///     Maps to BluetoothScanMatchNumber.OneAdvertisement (value: 1).
    ///     Determines how many advertisements to match per filter, as this is scarce hw resource.
    /// </summary>
    /// <remarks>
    ///     Requires Android 6.0 (API 23) or higher. Ignored on lower API levels.
    ///     https://developer.android.com/reference/android/bluetooth/le/ScanSettings#MATCH_NUM_ONE_ADVERTISEMENT
    /// </remarks>
    One = 1,

    /// <summary>
    ///     Report after matching a few advertisements.
    ///     Maps to BluetoothScanMatchNumber.FewAdvertisement (value: 2).
    ///     Match few advertisement per filter, depends on current capability and availability of the resources in hw.
    /// </summary>
    /// <remarks>
    ///     Requires Android 6.0 (API 23) or higher. Ignored on lower API levels.
    ///     https://developer.android.com/reference/android/bluetooth/le/ScanSettings#MATCH_NUM_FEW_ADVERTISEMENT
    /// </remarks>
    Few = 2,

    /// <summary>
    ///     Report after matching many advertisements.
    ///     Maps to BluetoothScanMatchNumber.MaxAdvertisement (value: 3).
    /// </summary>
    /// <remarks>
    ///     Requires Android 6.0 (API 23) or higher. Ignored on lower API levels.
    ///     https://developer.android.com/reference/android/bluetooth/le/ScanSettings#MATCH_NUM_MAX_ADVERTISEMENT
    /// </remarks>
    Max = 3
}

#pragma warning restore CA1008 // Enums should have zero value
