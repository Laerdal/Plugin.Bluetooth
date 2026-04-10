namespace Bluetooth.Avalonia.Platforms.Windows.Scanning;

/// <summary>
///     Extension methods for registering Windows BLE scanning services for Avalonia applications.
/// </summary>
public static class ScanningServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Windows BLE scanning services to the service collection.
    /// </summary>
    public static void AddBluetoothAvaloniaWindowsScanningServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothRemoteDeviceFactory, WindowsBluetoothRemoteDeviceFactory>();
    }
}
