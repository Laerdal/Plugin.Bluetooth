namespace Bluetooth.Avalonia.Platforms.Android.Scanning;

/// <summary>
///     Extension methods for registering Android BLE scanning services for Avalonia applications.
/// </summary>
public static class ScanningServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Android BLE scanning services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothAvaloniaAndroidScanningServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothRemoteDeviceFactory, AndroidBluetoothRemoteDeviceFactory>();
    }
}
