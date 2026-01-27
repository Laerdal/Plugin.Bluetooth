using Bluetooth.Core.Scanning.CharacteristicAccess;

namespace Bluetooth.Maui.Extensions;

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
    /// <param name="appleOptions"></param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static void AddBluetoothServices(this IServiceCollection services)
    {
#if IOS || MACCATALYST || ANDROID || WINDOWS
        services.AddSingleton<IBluetoothAdapter, BluetoothAdapter>();
        services.AddSingleton<IBluetoothCharacteristicAccessServicesRepository, CharacteristicAccessServicesRepository>();
        services.AddSingleton<IBluetoothPermissionManager, BluetoothPermissionManager>();
        services.AddSingleton<IBluetoothScanner, BluetoothScanner>();
        services.AddSingleton<IBluetoothBroadcaster, IBluetoothBroadcaster>();

        #if IOS || MACCATALYST
        services.ConfigureOptions<BluetoothAppleOptions>();

        #endif
        #if ANDROID
        services.AddSingleton<AdvertiseCallback, ()

        #endif
        #if WINDOWS

        #endif
#endif
    }
}
