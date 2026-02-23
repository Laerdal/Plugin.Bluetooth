using Bluetooth.Abstractions.Scanning.Options;

using ScanMode = Android.Bluetooth.LE.ScanMode;

namespace Bluetooth.Maui.Platforms.Droid.Tools;

/// <summary>
///     Provides extension methods for converting between abstract Bluetooth scan modes and Android scan modes.
/// </summary>
public static class ScanModeConverter
{
    /// <summary>
    ///     Converts an abstract <see cref="BluetoothScanMode" /> value to the corresponding Android <see cref="Android.Bluetooth.LE.ScanMode" /> value.
    /// </summary>
    /// <remarks>
    ///     Maps abstract scan mode preferences to native Android scan mode types.
    ///     The Opportunistic mode requires API 24+ and falls back to Balanced on lower API levels.
    /// </remarks>
    public static ScanMode ToAndroidScanMode(this BluetoothScanMode scanMode)
    {
        return scanMode switch
        {
            BluetoothScanMode.LowPower => ScanMode.LowPower,
            BluetoothScanMode.Balanced => ScanMode.Balanced,
            BluetoothScanMode.LowLatency => ScanMode.LowLatency,
            BluetoothScanMode.Opportunistic when OperatingSystem.IsAndroidVersionAtLeast(24) => ScanMode.Opportunistic,
            BluetoothScanMode.Opportunistic => ScanMode.Balanced, // Fallback for API < 24
            _ => ScanMode.Balanced
        };
    }
}
