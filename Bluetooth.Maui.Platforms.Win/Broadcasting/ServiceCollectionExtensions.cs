namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <summary>
///     Extension methods for registering Windows Bluetooth broadcasting services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Windows-specific Bluetooth broadcasting services to the service collection.
    ///     The Windows broadcaster performs direct platform-native entity creation internally.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddBluetoothMauiWindowsBroadcastingServices(this IServiceCollection services)
    {
        services.AddSingleton<IBluetoothBroadcaster, WindowsBluetoothBroadcaster>();
    }
}
