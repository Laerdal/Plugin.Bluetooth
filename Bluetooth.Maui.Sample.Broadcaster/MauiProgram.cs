namespace Bluetooth.Maui.Sample.Broadcaster;

/// <summary>
///     Main entry point for MAUI application configuration.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    ///     Creates and configures the MAUI application.
    /// </summary>
    /// <returns>The configured MAUI application.</returns>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        // Register Bluetooth services
        builder.Services.AddBluetoothServices();

        // Register app services
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Register pages and view models
        builder.Services.AddTransient<BroadcasterDemoPage>();
        builder.Services.AddTransient<BroadcasterDemoViewModel>();

        return builder.Build();
    }
}
