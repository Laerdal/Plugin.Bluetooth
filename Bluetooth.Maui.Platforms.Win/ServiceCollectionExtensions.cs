using Bluetooth.Maui.Platforms.Win.Broadcasting;
using Bluetooth.Maui.Platforms.Win.NativeObjects;
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
    /// <remarks>
    ///     Native object wrappers (BluetoothAdapterWrapper, RadioWrapper) are not registered in DI
    ///     as they require asynchronous initialization. They are created on-demand by WindowsBluetoothAdapter
    ///     via async getter methods.
    /// </remarks>
    public static void AddBluetoothMauiWindowsServices(this IServiceCollection services)
    {
        // Platform-specific services
        services.AddSingleton<IBluetoothAdapter, WindowsBluetoothAdapter>();

        // Note: BluetoothAdapterWrapper and RadioWrapper are NOT registered in DI
        // They are created lazily by WindowsBluetoothAdapter with async initialization

        // Register scanning and broadcasting factories
        services.AddBluetoothMauiWindowsScanningServices();
        services.AddBluetoothMauiWindowsBroadcastingServices();
    }
}
