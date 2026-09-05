using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for closest-device scan mode, intended for quick single-target discovery.
/// </summary>
public class ScanClosestDeviceViewModel : BaseScanViewModel
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScanClosestDeviceViewModel" /> class.
    /// </summary>
    public ScanClosestDeviceViewModel(IBluetoothScanner scanner,
        INavigationService navigation,
        ILogger<ScanClosestDeviceViewModel> logger)
        : base(scanner, navigation, logger)
    {
        OpenDeviceCommand = new AsyncRelayCommand(OpenDeviceAsync, () => HasClosestDevice);
    }

    protected override void RefreshUI(object? sender, EventArgs e)
    {
        if(Scanner.IsRunning)
        {
            var closest = Scanner.GetClosestDeviceOrDefault(DeviceFilter);

            ClosestDevice = closest;
            HasClosestDevice = closest != null;
            ClosestDeviceName = closest?.Name;
            ClosestDeviceId = closest?.Id;
            ClosestSignalStrengthInDbm = closest?.SignalStrengthInDbm ?? -127;
            ClosestSignalStrengthInPercent = closest?.SignalStrengthInPercent ?? 0.0;
        }
    }

    /// <summary>
    ///     Gets the closest currently discovered device.
    /// </summary>
    private IBluetoothRemoteDevice? ClosestDevice
    {
        get => GetValue<IBluetoothRemoteDevice?>(null);
        set => SetValue(value); 
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
    public int ClosestSignalStrengthInDbm
    {
        get => GetValueOrDefault(-127);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets the closest device RSSI progress in the 0..1 range.
    /// </summary>
    public double ClosestSignalStrengthInPercent
    {
        get => GetValueOrDefault(0.0d);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets command to open device details for the closest device.
    /// </summary>
    public IAsyncRelayCommand OpenDeviceCommand { get; }

    private async Task OpenDeviceAsync()
    {
        if (ClosestDevice == null)
        {
            return;
        }

        await Navigation.NavigateToAsync<DevicePage>(new Dictionary<string, object>
        {
            ["Device"] = ClosestDevice
        });
    }

    protected override void UpdateStatus()
    {
        if (!Scanner.IsRunning)
        {
            ScanStatus = "⚫ Scan is stopped";
            return;
        }

        if (ClosestDevice == null)
        {
            ScanStatus = "🔵 Scanning... bring your device closer";
            return;
        }

        ScanStatus = "🟢 Closest device identified";
    }
}
