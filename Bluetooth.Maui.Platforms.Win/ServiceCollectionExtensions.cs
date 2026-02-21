using Bluetooth.Maui.Platforms.Win.Broadcasting;
using Bluetooth.Maui.Platforms.Win.NativeObjects;
using Bluetooth.Maui.Platforms.Win.Permissions;
using Bluetooth.Maui.Platforms.Win.Scanning;

namespace Bluetooth.Maui.Platforms.Win;

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
        services.AddSingleton<IBluetoothAdapter, WindowsBluetoothAdapter>();
        services.AddSingleton<IBluetoothPermissionManager, BluetoothPermissionManager>();
        
        services.AddSingleton<IBluetoothAdapterWrapper, BluetoothAdapterWrapper>();
        services.AddSingleton<IRadioWrapper, RadioWrapper>();

        // Register scanning and broadcasting factories
        services.AddBluetoothMauiWindowsScanningServices();
        services.AddBluetoothMauiWindowsBroadcastingServices();
    }
}