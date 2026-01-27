using Bluetooth.Maui.PlatformSpecific.Permissions;

namespace Bluetooth.Maui;

/// <summary>
/// Provides iOS-specific Bluetooth permissions for the Bluetooth.Maui library.
/// </summary>
public static class BluetoothPermissions
{
    /// <summary>
    /// Gets the permission required for always-on Bluetooth access on iOS.
    /// This permission allows the app to use Bluetooth even when running in the background.
    /// </summary>
    public static IosPermissionForBluetoothAlways BluetoothAlways { get; } = new IosPermissionForBluetoothAlways();

    /// <summary>
    /// Gets the permission required for Bluetooth peripheral mode on iOS.
    /// This permission allows the app to act as a Bluetooth peripheral device.
    /// </summary>
    public static IosPermissionForBluetoothPeripheral BluetoothPeripheral { get; } = new IosPermissionForBluetoothPeripheral();
}
