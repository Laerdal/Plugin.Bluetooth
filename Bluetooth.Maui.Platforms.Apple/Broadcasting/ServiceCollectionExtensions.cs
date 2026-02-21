using Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <summary>
///     Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Bluetooth broadcaster services to the service collection.
    ///     Registers all core Bluetooth services including the adapter, scanner, broadcaster,
    ///     and characteristic access services as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static void AddBluetoothMauiAppleBroadcastingServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothLocalCharacteristicFactory, AppleBluetoothLocalCharacteristicFactory>();
        services.AddSingleton<IBluetoothLocalServiceFactory, AppleBluetoothLocalServiceFactory>();
        services.AddSingleton<IBluetoothLocalDescriptorFactory, AppleBluetoothLocalDescriptorFactory>();
        services.AddSingleton<IBluetoothConnectedDeviceFactory, AppleBluetoothConnectedClientDeviceFactory>();

        services.AddSingleton<CBPeripheralManagerDelegate, CbPeripheralManagerWrapper>();
        services.Configure<CbPeripheralManagerOptions>(options => {
            options.ShowPowerAlert = true;
            options.RestoreIdentifierKey = "com.bluetooth.maui.peripheralmanager.restore";
        });
    }
}