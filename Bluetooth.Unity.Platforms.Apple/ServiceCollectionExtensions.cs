using Bluetooth.Maui.Platforms.Apple.Broadcasting;
using Bluetooth.Maui.Platforms.Apple.Scanning;

namespace Bluetooth.Maui.Platforms.Apple;

/// <summary>
///     Extension methods for registering Bluetooth services in a Unity project's service collection.
/// </summary>
public static class UnityServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Unity-compatible Apple Bluetooth services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothUnityAppleServices(this IServiceCollection services)
    {
        services.Configure<DispatchQueueProviderOptions>(options => {
            options.CentralQueueLabel = "com.bluetooth.unity.central";
            options.PeripheralQueueLabel = "com.bluetooth.unity.peripheral";
        });

        services.AddSingleton<IDispatchQueueProvider, DispatchQueueProvider>();

        services.AddSingleton<IBluetoothAdapter, AppleBluetoothAdapter>();

        services.AddBluetoothMauiAppleScanningServices();
        services.AddBluetoothMauiAppleBroadcastingServices();
    }
}
