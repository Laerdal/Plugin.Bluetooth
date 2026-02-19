using Bluetooth.Maui.Platforms.Apple.Broadcasting;
using Bluetooth.Maui.Platforms.Apple.Permissions;
using Bluetooth.Maui.Platforms.Apple.Scanning;

namespace Bluetooth.Maui.Platforms.Apple;

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
    public static void AddBluetoothMauiAppleServices(this IServiceCollection services)
    {
        services.Configure<DispatchQueueProviderOptions>(options =>
        {
            options.CentralQueueLabel = "com.bluetooth.maui.central";
            options.PeripheralQueueLabel = "com.bluetooth.maui.peripheral";
        });

        services.AddSingleton<IDispatchQueueProvider, DispatchQueueProvider>();

        services.AddSingleton<IBluetoothAdapter, BluetoothAdapter>();
        services.AddSingleton<IBluetoothPermissionManager, BluetoothPermissionManager>();

        services.AddSingleton<IBluetoothScanner, AppleBluetoothScanner>();
        services.AddSingleton<IBluetoothBroadcaster, AppleBluetoothBroadcaster>();

        services.AddBluetoothMauiAppleScanningServices();
        services.AddBluetoothMauiAppleBroadcastingServices();
    }
}
