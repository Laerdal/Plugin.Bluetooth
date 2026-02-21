using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
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
        
        services.AddSingleton<IBluetoothCharacteristicFactory, AppleBluetoothCharacteristicFactory>();
        services.AddSingleton<IBluetoothServiceFactory, AppleBluetoothServiceFactory>();
        services.AddSingleton<IBluetoothDescriptorFactory, AppleBluetoothDescriptorFactory>();
        services.AddSingleton<IBluetoothDeviceFactory, AppleBluetoothDeviceFactory>();

        services.AddSingleton<CbCentralManagerWrapper>();
        services.Configure<CBCentralInitOptions>(options => {
            options.ShowPowerAlert = true;
            options.RestoreIdentifier = "com.bluetooth.maui.centralmanager.restore";
        });
    }
}