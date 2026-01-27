using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;

using Bluetooth.Core.Abstractions;
using Bluetooth.Core.Abstractions.Scan;
using Bluetooth.Maui.Sample.Services;
using Bluetooth.Maui.Sample.Views;
using Bluetooth.Maui.Sample.Views.Popup;

using CommunityToolkit.Mvvm.Input;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Maui.Sample.ViewModels;

/// <summary>
/// ViewModel for the Bluetooth scanner page
/// </summary>
public class ScannerPageViewModel : BaseViewModel
{
    private readonly BluetoothScannerService _scannerService;

    public bool IsScanning
    {
        get => GetValue(false);
        set => SetValue(value);
    }

    public ICommand OnStartScannerButtonClickedCommand { get; }

    public ICommand OnStopScannerButtonClickedCommand { get; }

    public ICommand OnConfigScannerButtonClickedCommand { get; }

    /// <summary>
    /// Observable collection of discovered Bluetooth devices
    /// </summary>
    public ObservableCollection<IBluetoothDevice> Devices => _scannerService.Devices;

    public IBluetoothDevice? SelectedDevice
    {
        get => GetValue<IBluetoothDevice?>(null);
        set
        {
            if (!SetValue(value) || value is null)
            {
                return;
            }
            NavigationService.NavigateTo<DevicePage>(new Dictionary<string, object>
            {
                { "deviceId", value.Id }
            });
        }
    }

    public ScannerPageViewModel(BluetoothScannerService scannerService, INavigationService navigationService) : base(navigationService)
    {
        _scannerService = scannerService ?? throw new ArgumentNullException(nameof(scannerService));

        _scannerService.Scanner.RunningStateChanged += OnScannerOnRunningStateChanged;

        OnStartScannerButtonClickedCommand = new AsyncRelayCommand(OnStartScannerButtonClickedAsync);
        OnStopScannerButtonClickedCommand = new AsyncRelayCommand(OnStopScannerButtonClickedAsync);
        OnConfigScannerButtonClickedCommand = new AsyncRelayCommand(OnConfigScannerButtonClickedAsync);
        Title = "Bluetooth Scanner";
        return;

        void OnScannerOnRunningStateChanged(object? s, EventArgs e)
        {
            IsScanning = _scannerService.Scanner.IsRunning;
        }
    }

    private async Task OnConfigScannerButtonClickedAsync()
    {
        await NavigationService.ShowPopupAsync<ScannerConfigPopup>();
    }

    private async Task OnStopScannerButtonClickedAsync()
    {
        Debug.WriteLine("Stop Scanner button clicked.");
        try
        {
            await _scannerService.Scanner.StopIfNeededAsync();
        }
        catch (Exception e)
        {
            App.DisplayAlert(e);
        }
    }

    private async Task OnStartScannerButtonClickedAsync()
    {
        Debug.WriteLine("Start Scanner button clicked.");
        try
        {
            await _scannerService.Scanner.StartIfNeededAsync();
        }
        catch (Exception e)
        {
            App.DisplayAlert(e);
        }
    }
}
