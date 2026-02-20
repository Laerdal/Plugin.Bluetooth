using Bluetooth.Maui.Platforms.Windows.Broadcasting;
using Bluetooth.Maui.Platforms.Windows.Permissions;
using Bluetooth.Maui.Platforms.Windows.Scanning;

namespace Bluetooth.Maui.Platforms.Windows;

/// <summary>
///     Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Bluetooth scanner and broadcaster services to the service collection.
    ///     Registers all core Bluetooth services including the adapter, scanner, broadcaster,
    ///     and characteristic access services as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static void AddBluetoothMauiWindowsServices(this IServiceCollection services)
    {
        // Core infrastructure services
        services.AddSingleton<ITicker, Ticker>();
        services.AddBluetoothCoreServices();
        services.AddBluetoothCoreScanningServices();
        services.AddBluetoothCoreBroadcastingServices();

        // Platform-specific services
        services.AddSingleton<IBluetoothAdapter, BluetoothAdapter>();
        services.AddSingleton<IBluetoothPermissionManager, BluetoothPermissionManager>();

        services.AddSingleton<IBluetoothScanner, Scanning.WindowsBluetoothScanner>();
        services.AddSingleton<IBluetoothBroadcaster, Broadcasting.WindowsBluetoothBroadcaster>();

        // Register scanning and broadcasting factories
        services.AddBluetoothMauiWindowsScanningServices();
        services.AddBluetoothMauiWindowsBroadcastingServices();
    }
}