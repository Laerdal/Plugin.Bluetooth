using Bluetooth.Maui.Platforms.Droid.Broadcasting;
using Bluetooth.Maui.Platforms.Droid.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Scanning;

namespace Bluetooth.Maui.Platforms.Droid;

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
    public static void AddBluetoothMauiAndroidServices(this IServiceCollection services)
    {
        // Platform-specific services
        services.AddSingleton<IBluetoothAdapter, AndroidBluetoothAdapter>();
        
        services.AddSingleton<IBluetoothAdapterWrapper, BluetoothAdapterWrapper>();
        services.AddSingleton<IBluetoothManagerWrapper, BluetoothManagerWrapper>();

        services.AddBluetoothMauiAndroidScanningServices();
        services.AddBluetoothMauiAndroidBroadcastingServices();
    }
}