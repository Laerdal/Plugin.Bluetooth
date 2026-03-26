namespace Bluetooth.Core.Scanning;

using Bluetooth.Core.Scanning.Profiles.BluetoothSig;

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
    public static void AddBluetoothCoreScanningServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Default Rssi to signal strength converter
        services.AddSingleton<IBluetoothRssiToSignalStrengthConverter, LinearRssiToSignalStrengthConverter>();

        // Profile registry and name provider
        services.AddSingleton<IBluetoothProfileRegistry>(serviceProvider =>
        {
            var registry = new BluetoothProfileRegistry();

            foreach (var registrar in serviceProvider.GetServices<BluetoothProfileRegistrar>())
            {
                registrar(registry);
            }

            return registry;
        });

        services.AddSingleton<IBluetoothNameProvider, ProfileNameProvider>();
    }

    /// <summary>
    ///     Adds Bluetooth SIG profile definitions to the profile registry pipeline.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddBluetoothSigProfiles(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<BluetoothProfileRegistrar>(_ => BatteryProfile.Register);
        return services;
    }
}
