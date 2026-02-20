using BluetoothPhy = Android.Bluetooth.BluetoothPhy;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Options;

/// <summary>
///     Android-specific Bluetooth connection options.
/// </summary>
public record ConnectionOptions : Abstractions.Scanning.Options.ConnectionOptions
{
    /// <summary>
    ///     Gets or sets the preferred PHY for the connection (Android 8.0+).
    /// </summary>
    public BluetoothPhy? PreferredPhy { get; set; }
}