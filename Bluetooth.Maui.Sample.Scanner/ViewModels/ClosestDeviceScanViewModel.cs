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
    private IDispatcherTimer _uiRefreshTicker;



    /// <summary>
    ///     Initializes a new instance of the <see cref="ClosestDeviceScanViewModel" /> class.
    /// </summary>
    public ClosestDeviceScanViewModel(IBluetoothScanner scanner,
        INavigationService navigation,
        ILogger<ClosestDeviceScanViewModel> logger)
        : base(logger)
    {
        _scanner = scanner;
        _navigation = navigation;
        _logger = logger;

        ScanStatus = "Ready to scan";

        StartScanCommand = new AsyncRelayCommand(StartScanAsync, () => !_scanner.IsRunning);
        StopScanCommand = new AsyncRelayCommand(StopScanAsync, () => _scanner.IsRunning);
        OpenDeviceCommand = new AsyncRelayCommand(OpenDeviceAsync, () => HasClosestDevice);

        _scanner.RunningStateChanged += OnRunningStateChanged;

        _uiRefreshTicker = Application.Current!.Dispatcher.CreateTimer();

        _uiRefreshTicker.Tick += RefreshUI;
        _uiRefreshTicker.Interval = TimeSpan.FromMilliseconds(33);
        _uiRefreshTicker.Start();
    }

    private void RefreshUI(object? sender, EventArgs e)
    {
        if(_scanner.IsRunning)
        {
            var closest = _scanner.GetClosestDeviceOrDefault(device => !string.IsNullOrWhiteSpace(device.Name))
                          ?? _scanner.GetClosestDeviceOrDefault();

            ClosestDevice = closest;
            HasClosestDevice = closest != null;
            ClosestDeviceName = closest?.Name;
            ClosestDeviceId = closest?.Id;
            ClosestSignalStrengthDbm = closest?.SignalStrengthDbm ?? -127;
            ClosestSignalStrengthPercent = closest?.SignalStrengthPercent ?? 0.0;
        }
    }

    /// <summary>
    ///     Gets the closest currently discovered device.
    /// </summary>
    public IBluetoothRemoteDevice? ClosestDevice
    {
        get => GetValue<IBluetoothRemoteDevice?>(null);
        private set => SetValue(value); 
    }

    /// <summary>
    ///     Gets whether a closest device is available.
    /// </summary>
    public bool HasClosestDevice
    {
        get => GetValueOrDefault(false);
        set
        {
            if (SetValue(value))
            {
                OpenDeviceCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    ///     Gets the closest device display name.
    /// </summary>
    public string? ClosestDeviceName
    {
        get => GetValueOrDefault<string?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets the closest device id.
    /// </summary>
    public string? ClosestDeviceId
    {
        get => GetValueOrDefault<string?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets the closest device RSSI.
    /// </summary>
    public int ClosestSignalStrengthDbm
    {
        get => GetValueOrDefault(-127);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets the closest device RSSI progress in the 0..1 range.
    /// </summary>
    public double ClosestSignalStrengthPercent
    {
        get => GetValueOrDefault(0.0d);
        set => SetValue(value);
    }

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
            await StartScanAsync();
        }
        else
        {
            UpdateStatus();
        }
    }

    /// <inheritdoc />
    public async override ValueTask OnDisappearingAsync()
    {
        await base.OnDisappearingAsync();

        if (_scanner.IsRunning)
        {
            await StopScanAsync();
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
