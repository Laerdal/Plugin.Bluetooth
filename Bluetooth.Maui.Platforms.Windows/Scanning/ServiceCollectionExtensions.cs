using Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <summary>
///     Extension methods for registering Windows Bluetooth scanning services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Windows-specific Bluetooth scanning services to the service collection.
    ///     Registers device, service, characteristic, and descriptor factories as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothMauiWindowsScanningServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothDeviceFactory, WindowsBluetoothDeviceFactory>();
        services.AddSingleton<IBluetoothServiceFactory, WindowsBluetoothServiceFactory>();
        services.AddSingleton<IBluetoothCharacteristicFactory, WindowsBluetoothCharacteristicFactory>();
        services.AddSingleton<IBluetoothDescriptorFactory, WindowsBluetoothDescriptorFactory>();
    }
}