namespace Bluetooth.Maui.Sample.Scanner;

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
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        builder.Services.AddLogging(configure => configure.AddDebug()
                                                            .SetMinimumLevel(LogLevel.Debug));
#endif

        // Register Bluetooth services
        builder.Services.AddBluetoothServices();

        // Register app services
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Register pages and view models
        builder.Services.AddTransient<ScanListPage>();
        builder.Services.AddTransient<ScanListViewModel>();
        builder.Services.AddTransient<DevicePage>();
        builder.Services.AddTransient<DeviceViewModel>();
        builder.Services.AddTransient<CharacteristicsPage>();
        builder.Services.AddTransient<CharacteristicsViewModel>();
        builder.Services.AddTransient<CharacteristicDetailPage>();
        builder.Services.AddTransient<CharacteristicDetailViewModel>();
        builder.Services.AddTransient<WriteListenLabPage>();
        builder.Services.AddTransient<WriteListenLabViewModel>();
        builder.Services.AddTransient<ScanClosestDevicePage>();
        builder.Services.AddTransient<ScanClosestDeviceViewModel>();

        return builder.Build();
    }
}
