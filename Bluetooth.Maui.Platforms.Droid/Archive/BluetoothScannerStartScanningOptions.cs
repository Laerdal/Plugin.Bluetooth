namespace Bluetooth.Maui;

public record BluetoothScannerStartScanningOptions : BaseBluetoothScannerStartScanningOptions
{
    /// <summary>
    /// https://developer.android.com/reference/android/bluetooth/le/ScanFilter
    /// </summary>
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


    public Android.Bluetooth.LE.ScanFilter[]? ToAndroidScanFilters()
    {
        return ScanFilters?.ToArray();
    }

    // ReSharper disable once CognitiveComplexity
    public Android.Bluetooth.LE.ScanSettings? ToAndroidScanSettings()
    {
        using var builder = new Android.Bluetooth.LE.ScanSettings.Builder();

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setScanMode(int)
        builder.SetScanMode(ScanMode);

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setCallbackType(int)
        if (CallbackType.HasValue && OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            builder.SetCallbackType((Android.Bluetooth.LE.ScanCallbackType)(int)CallbackType.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setMatchMode(int)
        if (MatchMode.HasValue && OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            builder.SetMatchMode(MatchMode.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setLegacy(boolean)
        if (Legacy.HasValue && OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            builder.SetLegacy(Legacy.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setReportDelay(long)
        if (ReportDelay.HasValue && OperatingSystem.IsAndroidVersionAtLeast(21))
        {
            builder.SetReportDelay((long)ReportDelay.Value.TotalMilliseconds);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setNumOfMatches(int)
        if (NumOfMatches.HasValue && OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            builder.SetNumOfMatches(NumOfMatches.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setPhy(int)
        if (Phy.HasValue && OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            builder.SetPhy(Phy.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setRssiThreshold(int)
        if (RssiThreshold.HasValue && OperatingSystem.IsAndroidVersionAtLeast(36, 1))
        {
            builder.SetRssiThreshold(RssiThreshold.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setScanType(int)
        if (ScanType.HasValue && OperatingSystem.IsAndroidVersionAtLeast(36, 1))
        {
            builder.SetScanType((int)ScanType.Value);
        }


        return builder.Build();
    }
}
