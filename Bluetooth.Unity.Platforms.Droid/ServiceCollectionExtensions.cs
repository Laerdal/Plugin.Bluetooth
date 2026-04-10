using Bluetooth.Maui.Platforms.Droid.Broadcasting;
using Bluetooth.Maui.Platforms.Droid.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Scanning;

namespace Bluetooth.Maui.Platforms.Droid;

/// <summary>
///     Extension methods for registering Bluetooth services in a Unity project's service collection.
/// </summary>
public static class UnityServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Unity-compatible Android Bluetooth services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothUnityAndroidServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothAdapter, AndroidBluetoothAdapter>();

        services.AddSingleton<IBluetoothAdapterWrapper, BluetoothAdapterWrapper>();
        services.AddSingleton<IBluetoothManagerWrapper, BluetoothManagerWrapper>();

        services.AddBluetoothMauiAndroidScanningServices();
        services.AddBluetoothMauiAndroidBroadcastingServices();
    }
}
