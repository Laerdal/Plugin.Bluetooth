namespace Plugin.Bluetooth.Maui.PlatformSpecific.NativeOptions;

public record StartScanningOptions
{
    public IEnumerable<ScanFilter>? ScanFilters { get; set; }

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setScanMode(int)
    /// </summary>
    public Android.Bluetooth.LE.ScanMode ScanMode { get; set; } = Android.Bluetooth.LE.ScanMode.Balanced;

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setCallbackType(int)
    /// </summary>
    public ScanCallbackType? CallbackType { get; set; } // Let Android decide (default behavior)

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setMatchMode(int)
    /// </summary>
    public BluetoothScanMatchMode? MatchMode { get; set; } // Let Android decide (default behavior)

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setLegacy(boolean)
    /// </summary>
    public bool? Legacy { get; set; } // Prefer modern scanning

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setReportDelay(long)
    /// </summary>
    public TimeSpan? ReportDelay { get; set; } // TimeSpan.FromMilliseconds(500); // Batch results every 0.5s

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setNumOfMatches(int)
    /// </summary>
    public int? NumOfMatches { get; set; } // No match filtering

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setPhy(int)
    /// </summary>
    public Android.Bluetooth.BluetoothPhy? Phy { get; set; } // Let Android decide (default behavior)

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setRssiThreshold(int)
    /// </summary>
    public int? RssiThreshold { get; set; } // Let Android decide (default behavior)

    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setScanType(int)
    /// </summary>
    public ScanType? ScanType { get; set; } // Let Android decide (default behavior)
}
