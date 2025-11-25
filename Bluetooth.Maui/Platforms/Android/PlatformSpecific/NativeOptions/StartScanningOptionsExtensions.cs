namespace Bluetooth.Maui.PlatformSpecific.NativeOptions;

public static class StartScanningOptionsExtensions
{
    public static Android.Bluetooth.LE.ScanFilter[]? ToNativeScanFilters(this StartScanningOptions? startScanningOptions)
    {
        return startScanningOptions?.ScanFilters?.ToArray();
    }

    // ReSharper disable once CognitiveComplexity
    public static Android.Bluetooth.LE.ScanSettings? ToNativeScanSettings(this StartScanningOptions? startScanningOptions)
    {
        if (startScanningOptions == null)
        {
            return null;
        }

        using var builder = new Android.Bluetooth.LE.ScanSettings.Builder();

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setScanMode(int)
        builder.SetScanMode(startScanningOptions.ScanMode);

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setCallbackType(int)
        if (startScanningOptions.CallbackType.HasValue && OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            builder.SetCallbackType((Android.Bluetooth.LE.ScanCallbackType)(int)startScanningOptions.CallbackType.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setMatchMode(int)
        if (startScanningOptions.MatchMode.HasValue && OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            builder.SetMatchMode(startScanningOptions.MatchMode.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setLegacy(boolean)
        if (startScanningOptions.Legacy.HasValue && OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            builder.SetLegacy(startScanningOptions.Legacy.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setReportDelay(long)
        if (startScanningOptions.ReportDelay.HasValue && OperatingSystem.IsAndroidVersionAtLeast(21))
        {
            builder.SetReportDelay((long)startScanningOptions.ReportDelay.Value.TotalMilliseconds);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setNumOfMatches(int)
        if (startScanningOptions.NumOfMatches.HasValue && OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            builder.SetNumOfMatches(startScanningOptions.NumOfMatches.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setPhy(int)
        if (startScanningOptions.Phy.HasValue && OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            builder.SetPhy(startScanningOptions.Phy.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setRssiThreshold(int)
        if (startScanningOptions.RssiThreshold.HasValue && OperatingSystem.IsAndroidVersionAtLeast(36, 1))
        {
            builder.SetRssiThreshold(startScanningOptions.RssiThreshold.Value);
        }

        // https://developer.android.com/reference/android/bluetooth/le/ScanSettings.Builder#setScanType(int)
        if (startScanningOptions.ScanType.HasValue && OperatingSystem.IsAndroidVersionAtLeast(36, 1))
        {
            builder.SetScanType((int)startScanningOptions.ScanType.Value);
        }


        return builder.Build();
    }

}
