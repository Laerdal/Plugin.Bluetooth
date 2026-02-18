using Bluetooth.Abstractions.Options;
using Bluetooth.Core.Infrastructure.Scheduling;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bluetooth.Core;

/// <summary>
/// Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Bluetooth core services to the service collection.
    /// Registers all core Bluetooth services including the ticker and infrastructure options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">Optional configuration action for BluetoothInfrastructureOptions.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddBluetoothCoreServices(
        this IServiceCollection services,
        Action<BluetoothInfrastructureOptions>? configureOptions = null)
    {
        services.AddTicker();

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.AddSingleton<IOptions<BluetoothInfrastructureOptions>>(
                Options.Create(new BluetoothInfrastructureOptions()));
        }

        return services;
    }
}
