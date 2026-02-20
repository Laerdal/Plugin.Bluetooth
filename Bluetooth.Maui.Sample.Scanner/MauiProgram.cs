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
            .ConfigureMauiHandlers(handlers => {
#if WINDOWS
                Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) => {
                    handler.PlatformView.SingleSelectionFollowsFocus = false;
                });

                Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(Pages.Controls.CategoryChart), (handler, view) => {
                    if (view is Pages.Controls.CategoryChart && handler.PlatformView is Microsoft.Maui.Platform.ContentPanel contentPanel)
                    {
                        contentPanel.IsTabStop = true;
                    }
                });
#endif
            })
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
        builder.Services.AddTransient<ScannerPage>();
        builder.Services.AddTransient<ScannerViewModel>();
        builder.Services.AddTransient<DevicePage>();
        builder.Services.AddTransient<DeviceViewModel>();
        builder.Services.AddTransient<CharacteristicsPage>();
        builder.Services.AddTransient<CharacteristicsViewModel>();

        return builder.Build();
    }
}