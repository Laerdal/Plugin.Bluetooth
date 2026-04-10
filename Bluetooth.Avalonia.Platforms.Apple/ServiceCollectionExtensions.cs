using Bluetooth.Avalonia.Platforms.Apple.Broadcasting;
using Bluetooth.Avalonia.Platforms.Apple.Scanning;
using Bluetooth.Avalonia.Platforms.Apple.Tools;

namespace Bluetooth.Avalonia.Platforms.Apple;

/// <summary>
///     Extension methods for registering Apple Bluetooth services in an Avalonia application.
///     Works for both iOS (<c>net10.0-ios</c>) and macOS native (<c>net10.0-macos</c>).
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Apple (iOS / macOS native) Bluetooth services to the service collection for Avalonia applications.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <remarks>
    ///     Uses CoreBluetooth on both platforms. macOS native Bluetooth support requires macOS 13 or later.
    /// </remarks>
    public static void AddBluetoothAvaloniaAppleServices(this IServiceCollection services)
    {
        services.Configure<DispatchQueueProviderOptions>(options => {
            options.CentralQueueLabel = "com.bluetooth.avalonia.central";
            options.PeripheralQueueLabel = "com.bluetooth.avalonia.peripheral";
        });

        services.AddSingleton<IDispatchQueueProvider, DispatchQueueProvider>();
        services.AddSingleton<IBluetoothAdapter, AppleBluetoothAdapter>();

        services.AddBluetoothAvaloniaAppleScanningServices();
        services.AddBluetoothAvaloniaAppleBroadcastingServices();
    }
}
