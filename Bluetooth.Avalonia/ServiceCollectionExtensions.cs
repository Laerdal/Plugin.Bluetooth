namespace Bluetooth.Avalonia;

/// <summary>
///     Extension methods for registering Bluetooth services in a service collection for Avalonia applications.
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
    ///         Client projects can inherit from these facades to add custom behaviour without
    ///         dealing with platform-specific conditional compilation.
    ///     </para>
    ///     <para>
    ///         Supported Avalonia platforms:
    ///         <list type="bullet">
    ///             <item>Android — native BLE via Android Bluetooth LE APIs</item>
    ///             <item>iOS — native BLE via CoreBluetooth</item>
    ///             <item>macOS — native BLE via CoreBluetooth (macOS 13+, not MacCatalyst)</item>
    ///             <item>Windows — native BLE via WinRT Bluetooth APIs</item>
    ///             <item>Linux — stub (BlueZ integration planned)</item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public static void AddBluetoothAvaloniaServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ITicker, Ticker>();
        services.AddBluetoothCoreServices();
        services.AddBluetoothSigProfiles();
        services.AddBluetoothCoreScanningServices();
        services.AddBluetoothCoreBroadcastingServices();

#if WINDOWS
        services.AddBluetoothAvaloniaWindowsServices();
#elif ANDROID
        services.AddBluetoothAvaloniaAndroidServices();
#elif IOS || MACOS
        services.AddBluetoothAvaloniaAppleServices();
#else
        services.AddBluetoothAvaloniaLinuxServices();
#endif

        // Register unified facade wrappers as the default implementations.
        // These allow client projects to inherit a single class across all platforms.
        services.AddSingleton<IBluetoothScanner, BluetoothScanner>();
        services.AddSingleton<IBluetoothBroadcaster, BluetoothBroadcaster>();
    }
}
