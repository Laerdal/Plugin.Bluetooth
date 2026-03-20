using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
///     Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Bluetooth scanner services to the service collection.
    ///     Registers all core Bluetooth services including the adapter, scanner, broadcaster,
    ///     and characteristic access services as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static void AddBluetoothMauiAppleScanningServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothScanner, AppleBluetoothScanner>();

        // Configure CBCentralManager options
        services.Configure<CBCentralInitOptions>(options => {
            options.ShowPowerAlert = true;
            options.RestoreIdentifier = "com.bluetooth.maui.centralmanager.restore";
        });

        // Note: CbCentralManagerWrapper is NOT registered in DI - it's created by AppleBluetoothScanner
        // because it needs the scanner as its delegate (circular dependency)
    }
}
