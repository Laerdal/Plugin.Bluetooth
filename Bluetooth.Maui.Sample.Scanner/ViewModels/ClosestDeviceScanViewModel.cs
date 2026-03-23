using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for closest-device scan mode, intended for quick single-target discovery.
/// </summary>
public class ClosestDeviceScanViewModel : BaseViewModel
{
    private readonly ILogger<ClosestDeviceScanViewModel> _logger;
    private readonly INavigationService _navigation;
    private readonly IBluetoothScanner _scanner;

    private bool _startedScanningOnThisPage;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ClosestDeviceScanViewModel" /> class.
    /// </summary>
    public ClosestDeviceScanViewModel(IBluetoothScanner scanner,
        INavigationService navigation,
        ILogger<ClosestDeviceScanViewModel> logger)
    {
        _scanner = scanner;
        _navigation = navigation;
        _logger = logger;

        ScanStatus = "Ready to scan";

        StartScanCommand = new AsyncRelayCommand(StartScanAsync, () => !_scanner.IsRunning && !_scanner.IsStarting);
        StopScanCommand = new AsyncRelayCommand(StopScanAsync, () => _scanner.IsRunning || _scanner.IsStarting);
        OpenDeviceCommand = new AsyncRelayCommand(OpenDeviceAsync, () => ClosestDevice != null);

        _scanner.RunningStateChanged += OnRunningStateChanged;
        _scanner.DeviceListChanged += OnDeviceListChanged;
    }

    /// <summary>
    ///     Gets the closest currently discovered device.
    /// </summary>
    public IBluetoothRemoteDevice? ClosestDevice
    {
        get => GetValue<IBluetoothRemoteDevice?>(null);
        private set
        {
            if (!SetValue(value))
            {
                return;
            }

            OnPropertyChanged(nameof(HasClosestDevice));
            OnPropertyChanged(nameof(ClosestDeviceName));
            OnPropertyChanged(nameof(ClosestDeviceId));
            OnPropertyChanged(nameof(ClosestSignalStrengthDbm));
            OnPropertyChanged(nameof(ClosestSignalStrengthPercent));
            OpenDeviceCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    ///     Gets whether a closest device is available.
    /// </summary>
    public bool HasClosestDevice => ClosestDevice != null;

    /// <summary>
    ///     Gets the closest device display name.
    /// </summary>
    public string ClosestDeviceName => string.IsNullOrWhiteSpace(ClosestDevice?.Name)
        ? "Unknown Device"
        : ClosestDevice.Name;

    /// <summary>
    ///     Gets the closest device id.
    /// </summary>
    public string ClosestDeviceId => ClosestDevice?.Id ?? "N/A";

    /// <summary>
    ///     Gets the closest device RSSI.
    /// </summary>
    public int ClosestSignalStrengthDbm => ClosestDevice?.SignalStrengthDbm ?? -127;

    /// <summary>
    ///     Gets the closest device RSSI progress in the 0..1 range.
    /// </summary>
    public double ClosestSignalStrengthPercent => ClosestDevice?.SignalStrengthPercent ?? 0.0;

    /// <summary>
    ///     Gets or sets scan status text.
    /// </summary>
    public string ScanStatus
    {
        get => GetValue(string.Empty);
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets command to start closest-device scan mode.
    /// </summary>
    public IAsyncRelayCommand StartScanCommand { get; }

    /// <summary>
    ///     Gets command to stop closest-device scan mode.
    /// </summary>
    public IAsyncRelayCommand StopScanCommand { get; }

    /// <summary>
    ///     Gets command to open device details for the closest device.
    /// </summary>
    public IAsyncRelayCommand OpenDeviceCommand { get; }

    /// <inheritdoc />
    public async override ValueTask OnAppearingAsync()
    {
        await base.OnAppearingAsync();

        if (!_scanner.IsRunning)
        {
            _startedScanningOnThisPage = true;
            await StartScanAsync();
        }
        else
        {
            RefreshClosestDevice();
            UpdateStatus();
        }
    }

    /// <inheritdoc />
    public async override ValueTask OnDisappearingAsync()
    {
        await base.OnDisappearingAsync();

        if (_startedScanningOnThisPage)
        {
            await StopScanAsync();
            _startedScanningOnThisPage = false;
        }
    }

    private async Task StartScanAsync()
    {
        try
        {
            var options = new ScanningOptions
            {
                IgnoreNamelessAdvertisements = false
            };

            await _scanner.StartScanningIfNeededAsync(options);
            RefreshClosestDevice();
            UpdateStatus();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start closest-device scan mode");
            ScanStatus = $"Start failed: {ex.Message}";
        }
    }

    private async Task StopScanAsync()
    {
        try
        {
            await _scanner.StopScanningIfNeededAsync();
            UpdateStatus();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop closest-device scan mode");
            ScanStatus = $"Stop failed: {ex.Message}";
        }
    }

    private async Task OpenDeviceAsync()
    {
        if (ClosestDevice == null)
        {
            return;
        }

        await _navigation.NavigateToAsync<DevicePage>(new Dictionary<string, object>
        {
            ["Device"] = ClosestDevice
        });
    }

    private void OnRunningStateChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            StartScanCommand.NotifyCanExecuteChanged();
            StopScanCommand.NotifyCanExecuteChanged();
            UpdateStatus();
        });
    }

    private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            RefreshClosestDevice();
            UpdateStatus();
        });
    }

    private void RefreshClosestDevice()
    {
        var closest = _scanner.GetClosestDeviceOrDefault(device => !string.IsNullOrWhiteSpace(device.Name))
                      ?? _scanner.GetClosestDeviceOrDefault();

        ClosestDevice = closest;
    }

    private void UpdateStatus()
    {
        if (!_scanner.IsRunning)
        {
            ScanStatus = "Scan is stopped";
            return;
        }

        if (ClosestDevice == null)
        {
            ScanStatus = "Scanning... bring your device closer";
            return;
        }

        ScanStatus = "Closest device identified";
    }
}
