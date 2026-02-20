namespace Bluetooth.Core.Infrastructure.Scheduling;

/// <summary>
///     Extension methods for adding ticker services to the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a ticker service to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the ticker to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddTicker(this IServiceCollection services)
    {
        services.AddOptions<TickerOptions>();
        services.AddSingleton<ITicker, Ticker>();
        return services;
    }
}