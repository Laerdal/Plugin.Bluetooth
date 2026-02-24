using Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <summary>
///     Extension methods for registering Windows Bluetooth broadcasting services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Windows-specific Bluetooth broadcasting services to the service collection.
    ///     Registers local service, characteristic, descriptor, and connected device factories as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothMauiWindowsBroadcastingServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothBroadcaster, WindowsBluetoothBroadcaster>();

        // Register factories
        services.AddSingleton<IBluetoothLocalServiceFactory, WindowsBluetoothLocalServiceFactory>();
        services.AddSingleton<IBluetoothLocalCharacteristicFactory, WindowsBluetoothLocalCharacteristicFactory>();
        services.AddSingleton<IBluetoothLocalDescriptorFactory, WindowsBluetoothLocalDescriptorFactory>();
        services.AddSingleton<IBluetoothConnectedDeviceFactory, WindowsBluetoothConnectedDeviceFactory>();
    }
}
