namespace Bluetooth.Avalonia.Platforms.Linux.Broadcasting;

/// <summary>
///     Extension methods for registering Linux BLE broadcasting services for Avalonia applications.
/// </summary>
public static class BroadcastingServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Linux BLE broadcasting services to the service collection.
    ///     All operations throw <see cref="PlatformNotSupportedException"/> until a BlueZ implementation is provided.
    /// </summary>
    public static void AddBluetoothAvaloniaLinuxBroadcastingServices(this IServiceCollection services)
    {
        // Placeholder: BlueZ-backed broadcasting services will be registered here.
    }
}
