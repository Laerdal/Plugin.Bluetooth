namespace Bluetooth.Unity;

/// <summary>
///     Provides a simple, DI-free way to access Bluetooth services from Unity's
///     <c>MonoBehaviour</c> lifecycle without requiring a full dependency-injection container.
/// </summary>
/// <remarks>
///     <para>
///         Call <see cref="Initialize"/> once (e.g., in <c>Awake()</c> or <c>Start()</c> on a
///         root <c>MonoBehaviour</c>) before accessing <see cref="Scanner"/> or
///         <see cref="Broadcaster"/>.
///     </para>
///     <para>
///         If your project already uses a DI framework (Zenject, VContainer,
///         <c>Microsoft.Extensions.DependencyInjection</c>, etc.), register Bluetooth services
///         via <see cref="ServiceCollectionExtensions.AddBluetoothUnityServices"/> instead
///         and inject <see cref="IBluetoothScanner"/> / <see cref="IBluetoothBroadcaster"/>
///         normally.
///     </para>
///     <example>
///     <code>
///     // In a MonoBehaviour:
///     void Awake()
///     {
///         BluetoothLocator.Initialize();
///     }
///
///     async void Start()
///     {
///         await BluetoothLocator.Scanner.StartScanningAsync();
///         BluetoothLocator.Scanner.DeviceDiscovered += OnDeviceDiscovered;
///     }
///     </code>
///     </example>
/// </remarks>
public static class BluetoothLocator
{
    private static IBluetoothScanner? _scanner;
    private static IBluetoothBroadcaster? _broadcaster;
    private static ServiceProvider? _serviceProvider;

    /// <summary>
    ///     Gets the Bluetooth scanner. Returns <see langword="null"/> until
    ///     <see cref="Initialize"/> has been called.
    /// </summary>
    public static IBluetoothScanner Scanner
        => _scanner ?? throw new InvalidOperationException(
            "BluetoothLocator is not initialized. Call BluetoothLocator.Initialize() first.");

    /// <summary>
    ///     Gets the Bluetooth broadcaster. Returns <see langword="null"/> until
    ///     <see cref="Initialize"/> has been called.
    /// </summary>
    public static IBluetoothBroadcaster Broadcaster
        => _broadcaster ?? throw new InvalidOperationException(
            "BluetoothLocator is not initialized. Call BluetoothLocator.Initialize() first.");

    /// <summary>
    ///     Gets a value indicating whether <see cref="Initialize"/> has been called.
    /// </summary>
    public static bool IsInitialized => _scanner != null;

    /// <summary>
    ///     Initializes the Bluetooth services using an internal
    ///     <see cref="Microsoft.Extensions.DependencyInjection.ServiceCollection"/>.
    ///     Safe to call multiple times; subsequent calls are ignored.
    /// </summary>
    /// <param name="loggerFactory">
    ///     Optional logger factory. When <see langword="null"/> (default) logging is disabled.
    ///     Pass a factory from your preferred logging framework to enable BLE diagnostics.
    /// </param>
    public static void Initialize(ILoggerFactory? loggerFactory = null)
    {
        if (IsInitialized)
        {
            return;
        }

        var services = new ServiceCollection();

        if (loggerFactory != null)
        {
            services.AddSingleton(loggerFactory);
        }

        services.AddBluetoothUnityServices();

        _serviceProvider = services.BuildServiceProvider();
        _scanner = _serviceProvider.GetRequiredService<IBluetoothScanner>();
        _broadcaster = _serviceProvider.GetRequiredService<IBluetoothBroadcaster>();
    }

    /// <summary>
    ///     Disposes all Bluetooth services and resets the locator.
    ///     Call this when your application is shutting down (e.g., <c>OnApplicationQuit()</c>).
    /// </summary>
    public static void Dispose()
    {
        _scanner = null;
        _broadcaster = null;
        _serviceProvider?.Dispose();
        _serviceProvider = null;
    }
}
