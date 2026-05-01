using Bluetooth.Linux.Broadcasting;
using Bluetooth.Linux.Scanning;
using Bluetooth.Linux.Scanning.Factories;

namespace Bluetooth.Linux;

/// <summary>
///     Extension methods for registering Linux Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds all Linux Bluetooth services to the service collection, including the adapter,
    ///     scanner, factory chain, and broadcasting stubs.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothLinuxServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothAdapter, LinuxBluetoothAdapter>();
        services.AddSingleton<IBluetoothBroadcaster, LinuxBluetoothBroadcaster>();

        services.AddSingleton<IBluetoothRemoteDescriptorFactory, LinuxBluetoothRemoteDescriptorFactory>();
        services.AddSingleton<IBluetoothRemoteCharacteristicFactory, LinuxBluetoothRemoteCharacteristicFactory>();
        services.AddSingleton<IBluetoothRemoteServiceFactory, LinuxBluetoothRemoteServiceFactory>();
        services.AddSingleton<IBluetoothRemoteDeviceFactory, LinuxBluetoothRemoteDeviceFactory>();

        services.AddSingleton<IBluetoothScanner, LinuxBluetoothScanner>();
    }
}
