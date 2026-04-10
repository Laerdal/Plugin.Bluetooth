namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <summary>
///     Extension methods for registering Linux BLE scanning services for Avalonia applications.
/// </summary>
public static class ScanningServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Linux BLE scanning services to the service collection.
    ///     All operations throw <see cref="PlatformNotSupportedException"/> until a BlueZ implementation is provided.
    /// </summary>
    public static void AddBluetoothAvaloniaLinuxScanningServices(this IServiceCollection services)
    {
        // Placeholder: BlueZ-backed scanning services will be registered here.
    }
}
