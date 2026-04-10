using Bluetooth.Maui.Platforms.Win.Broadcasting;
using Bluetooth.Maui.Platforms.Win.NativeObjects;
using Bluetooth.Maui.Platforms.Win.Scanning;

namespace Bluetooth.Maui.Platforms.Win;

/// <summary>
///     Extension methods for registering Bluetooth services in a Unity project's service collection.
/// </summary>
public static class UnityServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Unity-compatible Windows Bluetooth services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothUnityWindowsServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothAdapter, WindowsBluetoothAdapter>();

        services.AddBluetoothMauiWindowsScanningServices();
        services.AddBluetoothMauiWindowsBroadcastingServices();
    }
}
