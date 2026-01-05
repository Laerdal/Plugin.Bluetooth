using Bluetooth.Maui.Sample.Services;
using Bluetooth.Maui.Sample.Views;

namespace Bluetooth.Maui.Sample.ViewModels;

public class DevicePageViewModel : BaseViewModel
{
    public BluetoothScannerService BluetoothScannerService { get; }

    public DevicePageViewModel(INavigationService navigationService, BluetoothScannerService bluetoothScannerService) : base(navigationService)
    {
        BluetoothScannerService = bluetoothScannerService;
    }


    public override void OnNavigatingToParameters (IDictionary<string, object>? parameters = null)
    {
        if (parameters == null)
        {
            return;
        }

        if (parameters.TryGetValue("deviceId", out var deviceIdObj) && deviceIdObj is string deviceId)
        {

        }
    }
}
