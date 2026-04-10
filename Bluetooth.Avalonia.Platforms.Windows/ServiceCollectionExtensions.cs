using Bluetooth.Avalonia.Platforms.Windows.Broadcasting;
using Bluetooth.Avalonia.Platforms.Windows.Scanning;

namespace Bluetooth.Avalonia.Platforms.Windows;

/// <summary>
///     Extension methods for registering Windows Bluetooth services in an Avalonia application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Windows Bluetooth services to the service collection for Avalonia applications.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothAvaloniaWindowsServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothAdapter, WindowsBluetoothAdapter>();

        services.AddBluetoothAvaloniaWindowsScanningServices();
        services.AddBluetoothAvaloniaWindowsBroadcastingServices();
    }
}
