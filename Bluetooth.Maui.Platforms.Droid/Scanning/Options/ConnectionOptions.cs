using BluetoothPhy = Android.Bluetooth.BluetoothPhy;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Options;

/// <summary>
///     Android-specific Bluetooth connection options that extend the abstract options
///     with a resolved <see cref="BluetoothPhy"/> value for use during GATT connection.
/// </summary>
public record ConnectionOptions : Abstractions.Scanning.Options.ConnectionOptions
{
    /// <summary>
    ///     Gets or sets the preferred PHY for the connection (Android 8.0+).
    ///     Null means no PHY preference is set.
    /// </summary>
    public BluetoothPhy? PreferredPhy { get; set; }
}
