using Bluetooth.Maui.Platforms.DotNetCore.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Scanning;

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
    public static void AddBluetoothMauiDotNetScanningServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothScanner, DotNetCoreBluetoothScanner>();
        services.AddSingleton<IBluetoothDeviceFactory, DotNetCoreBluetoothDeviceFactory>();
        services.AddSingleton<IBluetoothServiceFactory, DotNetCoreBluetoothServiceFactory>();
        services.AddSingleton<IBluetoothCharacteristicFactory, DotNetCoreBluetoothCharacteristicFactory>();
        services.AddSingleton<IBluetoothDescriptorFactory, DotNetCoreBluetoothDescriptorFactory>();


    }
}
