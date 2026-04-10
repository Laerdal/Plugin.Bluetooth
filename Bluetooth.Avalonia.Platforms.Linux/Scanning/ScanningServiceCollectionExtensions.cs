using Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <summary>
///     Extension methods for registering Linux BLE scanning services for Avalonia applications.
/// </summary>
public static class ScanningServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Linux BLE scanning services backed by BlueZ to the service collection.
    /// </summary>
    public static void AddBluetoothAvaloniaLinuxScanningServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothScanner, LinuxBluetoothScanner>();

        services.AddSingleton<IBluetoothRemoteDeviceFactory, LinuxBluetoothRemoteDeviceFactory>();
        services.AddSingleton<IBluetoothRemoteServiceFactory, LinuxBluetoothRemoteServiceFactory>();
        services.AddSingleton<IBluetoothRemoteCharacteristicFactory, LinuxBluetoothRemoteCharacteristicFactory>();
        services.AddSingleton<IBluetoothRemoteDescriptorFactory, LinuxBluetoothRemoteDescriptorFactory>();
    }
}
