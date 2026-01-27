using Bluetooth.Core.Scanning.CharacteristicAccess;

namespace Bluetooth.Maui;

/// <summary>
/// Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Bluetooth scanner and broadcaster services to the service collection.
    /// Registers all core Bluetooth services including the adapter, scanner, broadcaster,
    /// and characteristic access services as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static void AddMauiBluetoothServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothCharacteristicAccessServicesRepository, CharacteristicAccessServicesRepository>();
        services.AddSingleton<IBluetoothAdapter, BluetoothAdapter>();

        services.AddSingleton<IBluetoothScanner, BluetoothScanner>();
        services.AddSingleton<IBluetoothDeviceFactory, BluetoothDeviceFactory>();
        services.AddSingleton<IBluetoothServiceFactory, BluetoothServiceFactory>();
        services.AddSingleton<IBluetoothCharacteristicFactory, BluetoothCharacteristicFactory>();

        services.AddSingleton<IBluetoothBroadcaster, BluetoothBroadcaster>();
        services.AddSingleton<IBluetoothBroadcastClientDeviceFactory, BluetoothBroadcastClientDeviceFactory>();
        services.AddSingleton<IBluetoothBroadcastServiceFactory, BluetoothBroadcastServiceFactory>();
        services.AddSingleton<IBluetoothBroadcastCharacteristicFactory, BluetoothBroadcastCharacteristicFactory>();
    }
}
