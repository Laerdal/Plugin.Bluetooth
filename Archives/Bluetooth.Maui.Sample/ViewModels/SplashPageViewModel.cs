using System.Diagnostics;

using Bluetooth.Core.Abstractions;
using Bluetooth.Maui.Sample.Services;
using Bluetooth.Maui.Sample.Views;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Sample.ViewModels;

public class SplashPageViewModel : BaseViewModel
{

    public SplashPageViewModel(INavigationService navigationService) : base(navigationService)
    {
    }

    public override void OnViewNavigatedTo(Page baseContentPage, NavigatedToEventArgs args)
    {
        base.OnViewNavigatedTo(baseContentPage, args);
        NavigationService.NavigateTo<ScannerPage>(cleanStack: true);
    }

}
