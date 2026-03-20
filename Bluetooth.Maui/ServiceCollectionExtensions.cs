namespace Bluetooth.Maui;

/// <summary>
///     Extension methods for registering Bluetooth services in a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Bluetooth scanner and broadcaster services to the service collection.
    ///     Registers all core Bluetooth services including the adapter, scanner, broadcaster,
    ///     and characteristic access services as singletons.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    /// <remarks>
    ///     <para>
    ///         This method registers unified facade implementations (<see cref="BluetoothScanner"/>
    ///         and <see cref="BluetoothBroadcaster"/>) that wrap platform-specific implementations.
    ///         Client projects can inherit from these facades to add custom behavior without
    ///         dealing with platform-specific conditional compilation.
    ///     </para>
    /// </remarks>
    public static void AddBluetoothServices(this IServiceCollection services)
    {
        services.AddSingleton<ITicker, Ticker>();
        services.AddBluetoothCoreServices();
        services.AddBluetoothCoreScanningServices();
        services.AddBluetoothCoreBroadcastingServices();

#if WINDOWS
        services.AddBluetoothMauiWindowsServices();
#elif ANDROID
        services.AddBluetoothMauiAndroidServices();
#elif IOS || MACCATALYST
        services.AddBluetoothMauiAppleServices();
#else
        services.AddBluetoothMauiDotNetServices();
#endif

        // Register unified facade wrappers as the default implementations
        // These allow client projects to inherit a single class across all platforms
        services.AddSingleton<IBluetoothScanner, BluetoothScanner>();
        services.AddSingleton<IBluetoothBroadcaster, BluetoothBroadcaster>();
    }
}
