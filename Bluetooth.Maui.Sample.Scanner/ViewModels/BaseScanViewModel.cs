using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for closest-device scan mode, intended for quick single-target discovery.
/// </summary>
public abstract class BaseScanViewModel : BaseViewModel
{
    protected readonly INavigationService Navigation;
    protected readonly IBluetoothScanner Scanner;
    private IDispatcherTimer _uiRefreshTicker;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseScanViewModel" /> class.
    /// </summary>
    public BaseScanViewModel(IBluetoothScanner scanner,
        INavigationService navigation,
        ILogger<BaseScanViewModel> logger)
        : base(logger)
    {
        Scanner = scanner;
        Navigation = navigation;

        ScanStatus = "Ready to scan";

        StartScanCommand = new AsyncRelayCommand(StartScanAsync, () => !Scanner.IsRunning);
        StopScanCommand = new AsyncRelayCommand(StopScanAsync, () => Scanner.IsRunning);
        ClearFiltersCommand = new RelayCommand(ClearFilters);

        Scanner.RunningStateChanged += OnRunningStateChanged;

        _uiRefreshTicker = Application.Current!.Dispatcher.CreateTimer();

        _uiRefreshTicker.Tick += RefreshUI;
        _uiRefreshTicker.Interval = TimeSpan.FromMilliseconds(33);
        _uiRefreshTicker.Start();
    }

    protected abstract void RefreshUI(object? sender, EventArgs e);


    #region Start/Stop

    /// <summary>
    ///     Gets command to start closest-device scan mode.
    /// </summary>
    public IAsyncRelayCommand StartScanCommand { get; }

    /// <summary>
    ///     Gets command to stop closest-device scan mode.
    /// </summary>
    public IAsyncRelayCommand StopScanCommand { get; }

    private async Task StartScanAsync()
    {
        try
        {
            var options = new ScanningOptions
            {
                IgnoreNamelessAdvertisements = false
            };

            await Scanner.StartScanningIfNeededAsync(options);
            UpdateStatus();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to start closest-device scan mode");
            ScanStatus = $"Start failed: {ex.Message}";
        }
    }

    private async Task StopScanAsync()
    {
        try
        {
            await Scanner.StopScanningIfNeededAsync();
            UpdateStatus();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to stop closest-device scan mode");
            ScanStatus = $"Stop failed: {ex.Message}";
        }
    }

    private void OnRunningStateChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            StartScanCommand.NotifyCanExecuteChanged();
            StopScanCommand.NotifyCanExecuteChanged();
            UpdateStatus();
        });
    }

    protected virtual void UpdateStatus()
    {
        if (!Scanner.IsRunning)
        {
            ScanStatus = "⚫ Scan is stopped";
            return;
        }

        ScanStatus = "🔵 Scanning... ";
    }

    public string ScanStatus
    {
        get => GetValue(string.Empty);
        protected set => SetValue(value);
    }

    #endregion

    #region Filter

    public string? DeviceNameFilter
    {
        get => GetValue<string?>(null);
        set
        {
            if (SetValue(value))
            {
                ApplyFilters();
            }
        }
    }

    public int? MinimumSignalStrengthInDbm
    {
        get => GetValue<int?>(null);
        set
        {
            if (SetValue(value))
            {
                OnPropertyChanged(nameof(MinimumSignalStrengthText));
                ApplyFilters();
            }
        }
    }

    /// <summary>
    ///     Gets the minimum RSSI threshold display text.
    /// </summary>
    public string MinimumSignalStrengthText => $">= {MinimumSignalStrengthInDbm} dBm";

    public bool HideUnnamedDevices
    {
        get => GetValue(true);
        set
        {
            if (SetValue(value))
            {
                ApplyFilters();
            }
        }
    }

    protected bool DeviceFilter(IBluetoothRemoteDevice device)
    {
        return (DeviceNameFilter is null || device.Name?.Contains(DeviceNameFilter) == true) &&
               (MinimumSignalStrengthInDbm is null || device.SignalStrengthInDbm >= MinimumSignalStrengthInDbm) &&
               (!HideUnnamedDevices || !string.IsNullOrEmpty(device.Name));
    }

    private void ApplyFilters()
    {
        if (DeviceNameFilter is not null)
        {
            Scanner.AdvertisementFilter = adv => adv.DeviceName?.Contains(DeviceNameFilter) ?? false;
        }

        if (MinimumSignalStrengthInDbm is not null)
        {
            Scanner.AdvertisementFilter = adv => adv.SignalStrengthInDBm >= MinimumSignalStrengthInDbm;
        }

        if (HideUnnamedDevices)
        {
            Scanner.AdvertisementFilter = adv => !string.IsNullOrEmpty(adv.DeviceName);
        }
    }
    #endregion

    #region Clear Filters

    private void ClearFilters()
    {
        DeviceNameFilter = null;
        MinimumSignalStrengthInDbm = null;
        HideUnnamedDevices = true;
        ApplyFilters();
    }

    /// <summary>
    ///     Command to reset all scanner filters to their defaults.
    /// </summary>
    public IRelayCommand ClearFiltersCommand { get; }

    #endregion
}
