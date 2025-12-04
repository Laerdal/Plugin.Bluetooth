using Microsoft.Extensions.DependencyInjection;

namespace Bluetooth.Maui.Extensions;

/// <summary>
/// Extension methods for configuring Bluetooth services in a MAUI application.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Registers Bluetooth scanner and broadcaster services with the MAUI dependency injection container.
    /// Services are registered as singletons and will be lazily initialized on first use.
    /// </summary>
    /// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
    /// <param name="configureScanner">Optional action to configure the scanner after initialization.</param>
    /// <param name="configureBroadcaster">Optional action to configure the broadcaster after initialization.</param>
    /// <returns>The <see cref="MauiAppBuilder"/> for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This extension registers factory methods that create and initialize Bluetooth services asynchronously.
    /// The actual initialization (including permission checks) happens when the service is first resolved from DI.
    /// </para>
    /// <para>
    /// Example usage in MauiProgram.cs:
    /// <code>
    /// public static MauiApp CreateMauiApp()
    /// {
    ///     var builder = MauiApp.CreateBuilder();
    ///     builder.UseBluetooth(
    ///         configureScanner: scanner => scanner.AdvertisementFilter = ad => ad.IsConnectable,
    ///         configureBroadcaster: broadcaster => { /* configure */ }
    ///     );
    ///     return builder.Build();
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Then inject in your pages/view models:
    /// <code>
    /// public MainPage(IBluetoothScanner scanner)
    /// {
    ///     _scanner = scanner;
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// <strong>Important:</strong> The scanner/broadcaster initialization is async and may trigger permission requests.
    /// Consider calling initialization explicitly in your app startup if you need to handle permissions early:
    /// <code>
    /// // In App.xaml.cs or MainPage
    /// var scanner = serviceProvider.GetRequiredService&lt;IBluetoothScanner&gt;();
    /// await scanner.InitializeAsync(); // Explicitly initialize to handle permissions
    /// </code>
    /// </para>
    /// </remarks>
    public static MauiAppBuilder UseBluetooth(
        this MauiAppBuilder builder,
        Action<IBluetoothScanner>? configureScanner = null,
        Action<IBluetoothBroadcaster>? configureBroadcaster = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Register scanner factory - uses GetOrCreateDefaultScannerAsync pattern
        builder.Services.AddSingleton<IBluetoothScanner>(serviceProvider =>
        {
            // Note: This blocks on async initialization. For better control,
            // users should call InitializeAsync() explicitly in their app startup
            var scanner = BluetoothScanner.GetOrCreateDefaultScannerAsync()
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            configureScanner?.Invoke(scanner);
            return scanner;
        });

        // Register broadcaster factory
        builder.Services.AddSingleton<IBluetoothBroadcaster>(serviceProvider =>
        {
            var broadcaster = BluetoothBroadcaster.GetOrCreateDefaultBroadcasterAsync()
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            configureBroadcaster?.Invoke(broadcaster);
            return broadcaster;
        });

        return builder;
    }

    /// <summary>
    /// Registers only the Bluetooth scanner service with the MAUI dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
    /// <param name="configure">Optional action to configure the scanner after initialization.</param>
    /// <returns>The <see cref="MauiAppBuilder"/> for method chaining.</returns>
    /// <remarks>
    /// Use this if you only need scanning functionality and not broadcasting.
    /// See <see cref="UseBluetooth"/> for full documentation and usage examples.
    /// </remarks>
    public static MauiAppBuilder UseBluetoothScanner(
        this MauiAppBuilder builder,
        Action<IBluetoothScanner>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton<IBluetoothScanner>(serviceProvider =>
        {
            var scanner = BluetoothScanner.GetOrCreateDefaultScannerAsync()
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            configure?.Invoke(scanner);
            return scanner;
        });

        return builder;
    }

    /// <summary>
    /// Registers only the Bluetooth broadcaster service with the MAUI dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
    /// <param name="configure">Optional action to configure the broadcaster after initialization.</param>
    /// <returns>The <see cref="MauiAppBuilder"/> for method chaining.</returns>
    /// <remarks>
    /// Use this if you only need broadcasting/peripheral functionality and not scanning.
    /// See <see cref="UseBluetooth"/> for full documentation and usage examples.
    /// Note: Broadcasting is currently only fully implemented on Android.
    /// </remarks>
    public static MauiAppBuilder UseBluetoothBroadcaster(
        this MauiAppBuilder builder,
        Action<IBluetoothBroadcaster>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton<IBluetoothBroadcaster>(serviceProvider =>
        {
            var broadcaster = BluetoothBroadcaster.GetOrCreateDefaultBroadcasterAsync()
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            configure?.Invoke(broadcaster);
            return broadcaster;
        });

        return builder;
    }
}
