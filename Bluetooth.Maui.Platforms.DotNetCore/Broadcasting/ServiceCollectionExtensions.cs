using Bluetooth.Abstractions.Broadcasting;
using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.DotNetCore.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <summary>
/// Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Bluetooth broadcaster services to the service collection.
    /// Registers all core Bluetooth services including the adapter, scanner, broadcaster,
    /// and characteristic access services as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static void AddBluetoothMauiDotNetBroadcastingServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothBroadcaster, BluetoothBroadcaster>();
        services.AddSingleton<IBluetoothLocalCharacteristicFactory, BluetoothLocalCharacteristicFactory>();
        services.AddSingleton<IBluetoothLocalServiceFactory, BluetoothLocalServiceFactory>();
        services.AddSingleton<IBluetoothLocalDescriptorFactory, BluetoothLocalDescriptorFactory>();
        services.AddSingleton<IBluetoothConnectedDeviceFactory, BluetoothConnectedClientDeviceFactory>();
    }
}
