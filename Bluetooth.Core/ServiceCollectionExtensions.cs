using Bluetooth.Abstractions.Options;

namespace Bluetooth.Core;

/// <summary>
///     Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Bluetooth core services to the service collection.
    ///     Registers all core Bluetooth services including the ticker and infrastructure options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddBluetoothCoreServices(
        this IServiceCollection services)
    {
        services.AddTicker();
        return services;
    }
}
