using Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;
using Bluetooth.Maui.Platforms.DotNetCore.Scanning;

namespace Bluetooth.Maui.Platforms.DotNetCore;

/// <summary>
///     Extension methods for registering Bluetooth services in a Unity project's service collection.
///     This is the fallback implementation for platforms not explicitly supported.
/// </summary>
public static class UnityServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the default (stub) Bluetooth services to the service collection.
    ///     These stubs throw <see cref="PlatformNotSupportedException"/> at runtime.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothUnityDefaultServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothAdapter, DotNetBluetoothAdapter>();
        services.AddSingleton<IBluetoothBroadcaster, DotNetCoreBluetoothBroadcaster>();
        services.AddSingleton<IBluetoothScanner, DotNetCoreBluetoothScanner>();
    }
}
