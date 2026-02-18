using Bluetooth.Core.Infrastructure.Scheduling;

using Microsoft.Extensions.DependencyInjection;

namespace Bluetooth.Core.Scanning.CharacteristicAccess;

/// <summary>
/// Extension methods for adding ticker services to the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a ticker service to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the ticker to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddServiceDefinitions(this IServiceCollection services)
    {
        // Default Characteristic Access Services Repository
        services.AddSingleton<IBluetoothCharacteristicAccessServicesRepository, CharacteristicAccessServicesRepository>();
        
        return services;
    }
}
