using Bluetooth.Abstractions.Scanning.Options;

using ScanCallbackType = Android.Bluetooth.LE.ScanCallbackType;

namespace Bluetooth.Maui.Platforms.Droid.Tools;

/// <summary>
///     Provides extension methods for converting between abstract Bluetooth scan callback types and Android scan callback types.
/// </summary>
public static class ScanCallbackTypeConverter
{
    /// <summary>
    ///     Converts an abstract <see cref="BluetoothScanCallbackType" /> value to the corresponding Android <see cref="Android.Bluetooth.LE.ScanCallbackType" /> value.
    /// </summary>
    /// <remarks>
    ///     Maps abstract callback type preferences to native Android callback type flags.
    ///     Requires Android 6.0 (API 23) or higher.
    /// </remarks>
    public static ScanCallbackType ToAndroidScanCallbackType(this BluetoothScanCallbackType callbackType)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            throw new PlatformNotSupportedException("Callback type configuration requires Android 6.0 (API 23) or higher");
        }

        return callbackType switch
        {
            BluetoothScanCallbackType.AllMatches => ScanCallbackType.AllMatches,
            BluetoothScanCallbackType.FirstMatch => ScanCallbackType.FirstMatch,
            BluetoothScanCallbackType.MatchLost => ScanCallbackType.MatchLost,
            BluetoothScanCallbackType.FirstMatchAndMatchLost => ScanCallbackType.FirstMatch | ScanCallbackType.MatchLost,
            _ => ScanCallbackType.AllMatches
        };
    }
}
