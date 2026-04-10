namespace Bluetooth.Avalonia.Platforms.Apple.Scanning;

/// <summary>
///     Extension methods for registering Apple BLE scanning services for Avalonia applications.
/// </summary>
public static class ScanningServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Apple BLE scanning services to the service collection.
    ///     Works for both iOS and macOS native targets.
    /// </summary>
    public static void AddBluetoothAvaloniaAppleScanningServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothRemoteDeviceFactory, AppleBluetoothRemoteDeviceFactory>();
    }
}
