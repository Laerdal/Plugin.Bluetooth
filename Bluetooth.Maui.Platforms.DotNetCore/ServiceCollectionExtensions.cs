using Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;
using Bluetooth.Maui.Platforms.DotNetCore.Scanning;

namespace Bluetooth.Maui.Platforms.DotNetCore;

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
    public static void AddBluetoothMauiDotNetServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothAdapter, DotNetBluetoothAdapter>();

        services.AddBluetoothMauiDotNetScanningServices();
        services.AddBluetoothMauiDotNetBroadcastingServices();
    }
}