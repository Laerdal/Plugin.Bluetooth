
namespace Bluetooth.Maui;

/// <summary>
/// Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Bluetooth scanner and broadcaster services to the service collection.
    /// Registers all core Bluetooth services including the adapter, scanner, broadcaster,
    /// and characteristic access services as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static void AddBluetoothServices(this IServiceCollection services)
    {
        services.AddBluetoothCoreServices();
        services.AddBluetoothCoreScanningServices();
        services.AddBluetoothCoreBroadcastingServices();

#if WINDOWS
        services.AddBluetoothMauiWindowsServices();
#elif ANDROID
        services.AddBluetoothMauiAndroidServices();
#elif IOS || MACCATALYST
        services.AddBluetoothMauiAppleServices();
#else
        services.AddBluetoothMauiDotNetServices();
#endif
    }
}
