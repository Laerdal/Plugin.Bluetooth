using Bluetooth.Avalonia.Platforms.Android.Broadcasting;
using Bluetooth.Avalonia.Platforms.Android.Scanning;

namespace Bluetooth.Avalonia.Platforms.Android;

/// <summary>
///     Extension methods for registering Android Bluetooth services in an Avalonia application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Android Bluetooth services to the service collection for Avalonia applications.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothAvaloniaAndroidServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothAdapter, AndroidBluetoothAdapter>();

        services.AddBluetoothAvaloniaAndroidScanningServices();
        services.AddBluetoothAvaloniaAndroidBroadcastingServices();
    }
}
