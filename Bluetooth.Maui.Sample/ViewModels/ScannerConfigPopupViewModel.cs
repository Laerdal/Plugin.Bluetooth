using System.Windows.Input;

using Bluetooth.Maui.Sample.Services;

using CommunityToolkit.Mvvm.Input;

namespace Bluetooth.Maui.Sample.ViewModels;

public class ScannerConfigPopupViewModel : BaseViewModel
{
    private readonly BluetoothScannerService _scannerService;

    public bool IncludeUnnamedDevices
    {
        get => GetValue(_scannerService.IncludeUnnamedDevices);
        set
        {
            if(SetValue(value))
            {
                _scannerService.IncludeUnnamedDevices = value;
            }
        }
    }

    public ICommand OnCloseButtonClickedCommand => new AsyncRelayCommand(OnCloseButtonClickedAsync);

    private async Task OnCloseButtonClickedAsync()
    {
        await NavigationService.ClosePopupAsync();
    }

    public ScannerConfigPopupViewModel(INavigationService navigationService, BluetoothScannerService scannerService) : base(navigationService)
    {
        _scannerService = scannerService;
    }
}
