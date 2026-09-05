using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for the scanner page, handling BLE device discovery and display.
/// </summary>
public class ScanListViewModel : BaseScanViewModel
{

    /// <summary>
    ///     Collection of discovered Bluetooth devices.
    ///     Automatically updated when devices are discovered or removed.
    /// </summary>
    public ObservableCollection<IBluetoothRemoteDevice> Devices { get; } = new ObservableCollection<IBluetoothRemoteDevice>();


    /// <summary>
    ///     Initializes a new instance of the <see cref="ScanListViewModel" /> class.
    /// </summary>
    public ScanListViewModel(IBluetoothScanner scanner,
        INavigationService navigation,
        ILogger<ScanListViewModel> logger)
        : base(scanner, navigation, logger)
    {
        SelectDeviceCommand = new AsyncRelayCommand<IBluetoothRemoteDevice>(SelectDeviceAsync);
        OpenClosestDeviceScanCommand = new AsyncRelayCommand(OpenClosestDeviceScanAsync);
    }

    protected override void RefreshUI(object? sender, EventArgs e)
    {
        if (Scanner.IsRunning)
        {
            var devices = Scanner.GetDevices(DeviceFilter);

            Devices.UpdateFrom(devices);
            DeviceCount = Devices.Count;
        }
    }

    /// <summary>
    ///     Gets the number of discovered devices.
    /// </summary>
    public int DeviceCount
    {
        get => GetValueOrDefault(Devices.Count);
        protected set => SetValue(value);
    }

    #region Select device

    /// <summary>
    ///     Command to select a device from the list.
    /// </summary>
    public IAsyncRelayCommand<IBluetoothRemoteDevice> SelectDeviceCommand { get; }

    /// <summary>
    ///     Gets or sets the currently selected Bluetooth device.
    /// </summary>
    public IBluetoothRemoteDevice? SelectedDevice
    {
        get => GetValue<IBluetoothRemoteDevice?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Handles device selection and navigates to device details.
    /// </summary>
    /// <param name="device">The selected device.</param>
    private async Task SelectDeviceAsync(IBluetoothRemoteDevice? device)
    {
        if (device == null)
        {
            return;
        }

        Logger?.LogInformation("Device selected: {DeviceName} ({DeviceId})", device.Name ?? "Unknown", device.Id);

        SelectedDevice = null; // Clear selection after navigation to enable going back and forth
        // Navigate to DevicePage with the selected device
        await Navigation.NavigateToAsync<DevicePage>(new Dictionary<string, object>
        {
            ["Device"] = device
        });
    }

    #endregion

    #region Open closest-device scan

    /// <summary>
    ///     Command to open closest-device scan mode.
    /// </summary>
    public IAsyncRelayCommand OpenClosestDeviceScanCommand { get; }

    private async Task OpenClosestDeviceScanAsync()
    {
        await Navigation.NavigateToAsync<ScanClosestDevicePage>();
    }

    #endregion
}
