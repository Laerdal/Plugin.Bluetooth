namespace Bluetooth.Maui.Platforms.Droid.Tools;

/// <summary>
///     Provides extension methods for converting between Android Bluetooth PHY modes and shared PHY mode representations.
/// </summary>
public static class PhyModeConverter
{
    /// <summary>
    ///     Converts an Android <see cref="Android.Bluetooth.BluetoothPhy" /> value to the corresponding shared <see cref="PhyMode" /> value.
    /// </summary>
    public static PhyMode ToSharedPhyMode(this Android.Bluetooth.BluetoothPhy phy)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return PhyMode.None; // PHY modes are not supported on Android versions below 8.0 (API 26)
        }

        return phy switch
        {
            Android.Bluetooth.BluetoothPhy.Le1m => PhyMode.Le1M,
            Android.Bluetooth.BluetoothPhy.Le2m => PhyMode.Le2M,
            Android.Bluetooth.BluetoothPhy.LeCoded => PhyMode.LeCoded,
            _ => throw new ArgumentOutOfRangeException(nameof(phy), $"Unsupported PHY mode: {phy}")
        };
    }

    /// <summary>
    ///     Converts a shared <see cref="PhyMode" /> value to the corresponding Android <see cref="Android.Bluetooth.BluetoothPhy" /> value.
    /// </summary>
    public static Android.Bluetooth.BluetoothPhy ToAndroidPhyMode(this PhyMode phyMode)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            throw new PlatformNotSupportedException("PHY update requires Android 8.0 (API 26) or higher");
        }

        return phyMode switch
        {
            PhyMode.None => Android.Bluetooth.BluetoothPhy.Le1m, // Default to LE 1M if no preference is specified
            PhyMode.Le1M => Android.Bluetooth.BluetoothPhy.Le1m,
            PhyMode.Le2M => Android.Bluetooth.BluetoothPhy.Le2m,
            PhyMode.LeCoded => Android.Bluetooth.BluetoothPhy.LeCoded,
            _ => throw new ArgumentOutOfRangeException(nameof(phyMode), $"Unsupported PHY mode: {phyMode}")
        };
    }
}
