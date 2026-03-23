using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for the broadcaster demo page.
/// </summary>
public class BroadcasterDemoViewModel : BaseViewModel
{
    private static readonly Guid DemoServiceId = new("9D915819-EB1A-4F3F-8E07-369B6B02BE56");
    private static readonly Guid DemoCharacteristicId = new("E99644A7-671C-4A85-91AA-ECF24D6026E5");

    private readonly IBluetoothBroadcaster _broadcaster;
    private readonly ILogger<BroadcasterDemoViewModel> _logger;

    private IBluetoothLocalCharacteristic? _demoCharacteristic;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterDemoViewModel" /> class.
    /// </summary>
    public BroadcasterDemoViewModel(IBluetoothBroadcaster broadcaster, ILogger<BroadcasterDemoViewModel> logger)
    {
        _broadcaster = broadcaster;
        _logger = logger;

        LocalDeviceName = "Maui BLE Demo";
        PayloadInput = "Hello from BLE broadcaster";
        IncludeDeviceName = true;
        StatusMessage = "Ready";

        StartBroadcastingCommand = new AsyncRelayCommand(StartBroadcastingAsync, () => !_broadcaster.IsRunning && !_broadcaster.IsStarting);
        StopBroadcastingCommand = new AsyncRelayCommand(StopBroadcastingAsync, () => _broadcaster.IsRunning || _broadcaster.IsStarting);
        PushPayloadCommand = new AsyncRelayCommand(PushPayloadAsync, () => _demoCharacteristic != null);

        _broadcaster.RunningStateChanged += OnBroadcasterRunningStateChanged;
        _broadcaster.Starting += OnBroadcasterStateEvent;
        _broadcaster.Started += OnBroadcasterStateEvent;
        _broadcaster.Stopping += OnBroadcasterStateEvent;
        _broadcaster.Stopped += OnBroadcasterStateEvent;
    }

    /// <summary>
    ///     Gets whether the broadcaster is currently running.
    /// </summary>
    public bool IsRunning => _broadcaster.IsRunning;

    /// <summary>
    ///     Gets or sets the local device name to advertise when supported.
    /// </summary>
    public string LocalDeviceName
    {
        get => GetValue(string.Empty);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets whether to include local device name in advertisement.
    /// </summary>
    public bool IncludeDeviceName
    {
        get => GetValue(true);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets payload text used for characteristic value updates.
    /// </summary>
    public string PayloadInput
    {
        get => GetValue(string.Empty);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets status text for UI feedback.
    /// </summary>
    public string StatusMessage
    {
        get => GetValue(string.Empty);
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets the command to start broadcasting.
    /// </summary>
    public IAsyncRelayCommand StartBroadcastingCommand { get; }

    /// <summary>
    ///     Gets the command to stop broadcasting.
    /// </summary>
    public IAsyncRelayCommand StopBroadcastingCommand { get; }

    /// <summary>
    ///     Gets the command to push characteristic value updates.
    /// </summary>
    public IAsyncRelayCommand PushPayloadCommand { get; }

    /// <summary>
    ///     Gets timeline-style activity logs.
    /// </summary>
    public ObservableCollection<string> ActivityLog { get; } = new();

    private async Task StartBroadcastingAsync()
    {
        try
        {
            AppendLog("Preparing demo GATT service");
            await EnsureDemoServiceAsync().ConfigureAwait(false);

            var options = new BroadcastingOptions
            {
                IncludeDeviceName = IncludeDeviceName,
                LocalDeviceName = LocalDeviceName,
                AdvertisedServiceUuids = [DemoServiceId]
            };

            AppendLog("Starting broadcaster");
            await _broadcaster.StartBroadcastingIfNeededAsync(options).ConfigureAwait(false);

            StatusMessage = "Broadcasting";
            AppendLog("Broadcaster started");
            RefreshDerivedState();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start broadcaster demo");
            StatusMessage = $"Start failed: {ex.Message}";
            AppendLog(StatusMessage);
        }
    }

    private async Task StopBroadcastingAsync()
    {
        try
        {
            AppendLog("Stopping broadcaster");
            await _broadcaster.StopBroadcastingIfNeededAsync().ConfigureAwait(false);
            StatusMessage = "Stopped";
            AppendLog("Broadcaster stopped");
            RefreshDerivedState();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop broadcaster demo");
            StatusMessage = $"Stop failed: {ex.Message}";
            AppendLog(StatusMessage);
        }
    }

    private async Task PushPayloadAsync()
    {
        if (_demoCharacteristic == null)
        {
            StatusMessage = "Demo characteristic is not initialized yet.";
            AppendLog(StatusMessage);
            return;
        }

        try
        {
            var payload = string.IsNullOrWhiteSpace(PayloadInput)
                ? "Sample update"
                : PayloadInput;

            var bytes = Encoding.UTF8.GetBytes(payload);
            await _demoCharacteristic.UpdateValueAsync(bytes, notifyClients: true).ConfigureAwait(false);

            StatusMessage = $"Pushed {bytes.Length} bytes";
            AppendLog(StatusMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to push payload from broadcaster demo");
            StatusMessage = $"Push failed: {ex.Message}";
            AppendLog(StatusMessage);
        }
    }

    private async ValueTask EnsureDemoServiceAsync()
    {
        var service = _broadcaster.GetServiceOrDefault(DemoServiceId);
        if (service == null)
        {
            service = await _broadcaster.CreateServiceAsync(DemoServiceId, name: "Sample Demo Service").ConfigureAwait(false);
            AppendLog("Created sample service");
        }

        var characteristic = service.GetCharacteristicOrDefault(DemoCharacteristicId);
        if (characteristic == null)
        {
            characteristic = await service.CreateCharacteristicAsync(DemoCharacteristicId,
                BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Write | BluetoothCharacteristicProperties.Notify,
                BluetoothCharacteristicPermissions.Read | BluetoothCharacteristicPermissions.Write,
                name: "Sample Demo Characteristic").ConfigureAwait(false);

            AppendLog("Created sample characteristic");
        }

        _demoCharacteristic = characteristic;
        PushPayloadCommand.NotifyCanExecuteChanged();
    }

    private void OnBroadcasterRunningStateChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            RefreshDerivedState();
            StatusMessage = _broadcaster.IsRunning ? "Broadcasting" : "Stopped";
        });
    }

    private void OnBroadcasterStateEvent(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            var state = _broadcaster.IsStarting
                ? "Starting"
                : _broadcaster.IsStopping
                    ? "Stopping"
                    : _broadcaster.IsRunning
                        ? "Running"
                        : "Stopped";

            AppendLog($"State changed: {state}");
            RefreshDerivedState();
        });
    }

    private void RefreshDerivedState()
    {
        OnPropertyChanged(nameof(IsRunning));
        StartBroadcastingCommand.NotifyCanExecuteChanged();
        StopBroadcastingCommand.NotifyCanExecuteChanged();
    }

    private void AppendLog(string message)
    {
        var formatted = $"[{DateTime.Now:HH:mm:ss}] {message}";
        MainThread.BeginInvokeOnMainThread(() => {
            ActivityLog.Insert(0, formatted);
            while (ActivityLog.Count > 40)
            {
                ActivityLog.RemoveAt(ActivityLog.Count - 1);
            }
        });
    }
}
