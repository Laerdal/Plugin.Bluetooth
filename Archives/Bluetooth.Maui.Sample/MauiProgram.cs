using System.Collections.Concurrent;

using Bluetooth.Maui.Extensions;
using Bluetooth.Maui.Sample.Services;
using Bluetooth.Maui.Sample.ViewModels;
using Bluetooth.Maui.Sample.Views;
using Bluetooth.Maui.Sample.Views.Popup;

using CommunityToolkit.Maui;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

namespace Bluetooth.Maui.Sample;

public static class MauiProgram
{

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>()
               .UseMauiCommunityToolkit()
               .UseBluetoothScanner()
               .RegisterServices()
               .RegisterPages()
               .RegisterViewModels()
               .ConfigureFonts(fonts =>
               {
                   fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                   fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
               });

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.AddNLog();
#endif

        return builder.Build();
    }

    extension(MauiAppBuilder builder)
    {
        private MauiAppBuilder RegisterServices()
        {
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<BluetoothScannerService>();
            return builder;
        }

        private MauiAppBuilder RegisterPages()
        {
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<ScannerPage>();
            builder.Services.AddTransient<DevicePage>();
            builder.Services.AddTransient<ScannerConfigPopup>();
            return builder;
        }

        private MauiAppBuilder RegisterViewModels()
        {
            builder.Services.AddSingleton<SplashPageViewModel>();
            builder.Services.AddSingleton<ScannerPageViewModel>();
            builder.Services.AddSingleton<DevicePageViewModel>();
            builder.Services.AddTransient<ScannerConfigPopupViewModel>();
            return builder;
        }
    }

}
