using Plugin.BaseTypeExtensions;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
/// ViewModel for the scanner page, handling BLE device discovery and display.
/// </summary>
public class ScannerViewModel : BaseViewModel
{
    private readonly IBluetoothScanner _scanner;
    private readonly INavigationService _navigation;

    /// <summary>
    /// Collection of discovered Bluetooth devices.
    /// Automatically updated when devices are discovered or removed.
    /// </summary>
    public ObservableCollection<IBluetoothRemoteDevice> Devices { get; } = new();

    /// <summary>
    /// Gets a value indicating whether scanning is currently active.
    /// </summary>
    public bool IsScanning => GetValue<bool>();

    /// <summary>
    /// Gets the number of discovered devices.
    /// </summary>
    public int DeviceCount => Devices.Count;

    /// <summary>
    /// Command to start scanning for BLE devices.
    /// </summary>
    public IAsyncRelayCommand StartScanCommand { get; }

    /// <summary>
    /// Command to stop scanning for BLE devices.
    /// </summary>
    public IAsyncRelayCommand StopScanCommand { get; }

    /// <summary>
    /// Command to select a device from the list.
    /// </summary>
    public IAsyncRelayCommand<IBluetoothRemoteDevice> SelectDeviceCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScannerViewModel"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner service.</param>
    /// <param name="navigation">The navigation service.</param>
    public ScannerViewModel(IBluetoothScanner scanner, INavigationService navigation)
    {
        _scanner = scanner;
        _navigation = navigation;

        // Initialize commands
        StartScanCommand = new AsyncRelayCommand(StartScanAsync, () => !IsScanning);
        StopScanCommand = new AsyncRelayCommand(StopScanAsync, () => IsScanning);
        SelectDeviceCommand = new AsyncRelayCommand<IBluetoothRemoteDevice>(SelectDeviceAsync);

        // Subscribe to scanner events
        _scanner.RunningStateChanged += OnRunningStateChanged;
        _scanner.DeviceListChanged += OnDeviceListChanged;
    }

    /// <summary>
    /// Starts BLE scanning when the page appears.
    /// </summary>
    public override async ValueTask OnAppearingAsync()
    {
        await base.OnAppearingAsync();

        // Auto-start scanning when page appears (if not already scanning)
        if (!_scanner.IsRunning)
        {
            await StartScanAsync();
        }
    }

    /// <summary>
    /// Stops BLE scanning when the page disappears.
    /// </summary>
    public override async ValueTask OnDisappearingAsync()
    {
        await base.OnDisappearingAsync();

        // Stop scanning when page disappears to save battery
        if (_scanner.IsRunning)
        {
            await StopScanAsync();
        }
    }

    /// <summary>
    /// Starts scanning for BLE devices.
    /// </summary>
    private async Task StartScanAsync()
    {
        try
        {
            var options = new Bluetooth.Abstractions.Scanning.Options.ScanningOptions
            {
                // Optional: Configure scanning options here
                // For now, using defaults
            };

            await _scanner.StartScanningAsync(options);
        }
        catch (Exception ex)
        {
            // Handle scanning errors (permissions, Bluetooth off, etc.)
            await Application.Current!.MainPage!.DisplayAlert(
                "Scan Error",
                $"Failed to start scanning: {ex.Message}",
                "OK");
        }
    }

    /// <summary>
    /// Stops scanning for BLE devices.
    /// </summary>
    private async Task StopScanAsync()
    {
        try
        {
            await _scanner.StopScanningAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Stop Error",
                $"Failed to stop scanning: {ex.Message}",
                "OK");
        }
    }

    /// <summary>
    /// Handles device selection and navigates to device details.
    /// </summary>
    /// <param name="device">The selected device.</param>
    private async Task SelectDeviceAsync(IBluetoothRemoteDevice? device)
    {
        if (device == null) return;

        // For now, just show device info
        // In Phase 4, this will navigate to DevicePage
        await Application.Current!.MainPage!.DisplayAlert(
            "Device Selected",
            $"Name: {device.Name ?? "Unknown"}\n" +
            $"ID: {device.Id}\n" +
            $"RSSI: {device.SignalStrengthDbm} dBm\n" +
            $"Connected: {device.IsConnected}",
            "OK");

        // Phase 4 implementation (uncomment when DevicePage is ready):
        // await _navigation.NavigateToAsync<DevicePage>(new Dictionary<string, object>
        // {
        //     ["Device"] = device
        // });
    }

    /// <summary>
    /// Handles changes to the scanner's running state.
    /// </summary>
    private void OnRunningStateChanged(object? sender, EventArgs e)
    {
        SetValue(_scanner.IsRunning, nameof(IsScanning));
        StartScanCommand.NotifyCanExecuteChanged();
        StopScanCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Handles changes to the device list (devices added/removed).
    /// </summary>
    private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
    {
        // Update the ObservableCollection on the UI thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Use Plugin.BaseTypeExtensions to efficiently sync collections
            Devices.UpdateFrom(_scanner.GetDevices());
            OnPropertyChanged(nameof(DeviceCount));
        });
    }
}
