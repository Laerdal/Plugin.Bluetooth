using Bluetooth.Avalonia.Platforms.Linux.Broadcasting;
using Bluetooth.Avalonia.Platforms.Linux.Scanning;

namespace Bluetooth.Avalonia.Platforms.Linux;

/// <summary>
///     Extension methods for registering Bluetooth services in a service collection for Linux / Desktop Avalonia apps.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds stub Bluetooth services for Linux / plain .NET.
    ///     All operations will throw <see cref="PlatformNotSupportedException"/> until a BlueZ
    ///     implementation is provided.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothAvaloniaLinuxServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothAdapter, LinuxBluetoothAdapter>();
        services.AddSingleton<IBluetoothBroadcaster, LinuxBluetoothBroadcaster>();
        services.AddSingleton<IBluetoothScanner, LinuxBluetoothScanner>();
    }
}
