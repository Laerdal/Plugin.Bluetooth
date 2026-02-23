namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for the device detail page, handling device connection and service exploration.
/// </summary>
public class DeviceViewModel : BaseViewModel
{
    private readonly INavigationService _navigation;
    private IBluetoothRemoteDevice? _device;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceViewModel" /> class.
    /// </summary>
    public DeviceViewModel(INavigationService navigation)
    {
        _navigation = navigation;

        ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => !IsConnected);
        DisconnectCommand = new AsyncRelayCommand(DisconnectAsync, () => IsConnected);
        ExploreServicesCommand = new AsyncRelayCommand(ExploreServicesAsync, () => IsConnected);
        SelectServiceCommand = new AsyncRelayCommand<IBluetoothRemoteService>(SelectServiceAsync);
    }

    /// <summary>
    ///     Gets or sets the current Bluetooth device.
    /// </summary>
    public IBluetoothRemoteDevice? Device
    {
        get => _device;
        private set
        {
            if (_device != null)
            {
                _device.ConnectionStateChanged -= OnConnectionStateChanged;
                _device.ServiceListChanged -= OnServiceListChanged;
            }

            _device = value;

            if (_device != null)
            {
                _device.ConnectionStateChanged += OnConnectionStateChanged;
                _device.ServiceListChanged += OnServiceListChanged;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(DeviceName));
            OnPropertyChanged(nameof(DeviceId));
            OnPropertyChanged(nameof(IsConnected));
            UpdateCommands();
        }
    }

    /// <summary>
    ///     Gets the device name for display.
    /// </summary>
    public string DeviceName => Device?.Name ?? "Unknown Device";

    /// <summary>
    ///     Gets the device ID for display.
    /// </summary>
    public string DeviceId => Device?.Id ?? "N/A";

    /// <summary>
    ///     Gets whether the device is currently connected.
    /// </summary>
    public bool IsConnected => Device?.IsConnected ?? false;

    /// <summary>
    ///     Gets the collection of discovered services.
    /// </summary>
    public ObservableCollection<IBluetoothRemoteService> Services { get; } = new();

    /// <summary>
    ///     Gets the number of discovered services.
    /// </summary>
    public int ServiceCount => Services.Count;

    /// <summary>
    ///     Gets the command to connect to the device.
    /// </summary>
    public IAsyncRelayCommand ConnectCommand { get; }

    /// <summary>
    ///     Gets the command to disconnect from the device.
    /// </summary>
    public IAsyncRelayCommand DisconnectCommand { get; }

    /// <summary>
    ///     Gets the command to explore services on the connected device.
    /// </summary>
    public IAsyncRelayCommand ExploreServicesCommand { get; }

    /// <summary>
    ///     Gets the command to select and navigate to a service.
    /// </summary>
    public IAsyncRelayCommand<IBluetoothRemoteService> SelectServiceCommand { get; }

    /// <summary>
    ///     Sets the device to display and manage.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    public void SetDevice(IBluetoothRemoteDevice device)
    {
        Device = device;
    }

    /// <summary>
    ///     Connects to the device.
    /// </summary>
    private async Task ConnectAsync()
    {
        if (Device == null)
        {
            return;
        }

        try
        {
            // Create default connection options
            var connectionOptions = new ConnectionOptions();
            await Device.ConnectAsync(connectionOptions);
        }
        catch (Exception ex)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlertAsync(
                    "Connection Error",
                    $"Failed to connect: {ex.Message}",
                    "OK");
            }
        }
    }

    /// <summary>
    ///     Disconnects from the device.
    /// </summary>
    private async Task DisconnectAsync()
    {
        if (Device == null)
        {
            return;
        }

        try
        {
            await Device.DisconnectAsync();
            Services.Clear();
            OnPropertyChanged(nameof(ServiceCount));
        }
        catch (Exception ex)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlertAsync(
                    "Disconnection Error",
                    $"Failed to disconnect: {ex.Message}",
                    "OK");
            }
        }
    }

    /// <summary>
    ///     Explores and discovers services on the connected device.
    /// </summary>
    private async Task ExploreServicesAsync()
    {
        if (Device == null)
        {
            return;
        }

        try
        {
            // Explore services on the device
            await Device.ExploreServicesAsync();

            // Update the services collection on the UI thread
            MainThread.BeginInvokeOnMainThread(() => {
                Services.UpdateFrom([.. Device.GetServices()]);
                OnPropertyChanged(nameof(ServiceCount));
            });
        }
        catch (Exception ex)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlertAsync(
                    "Service Discovery Error",
                    $"Failed to discover services: {ex.Message}",
                    "OK");
            }
        }
    }

    /// <summary>
    ///     Navigates to the characteristics page for the selected service.
    /// </summary>
    private async Task SelectServiceAsync(IBluetoothRemoteService? service)
    {
        if (service == null)
        {
            return;
        }

        // Navigate to CharacteristicsPage with the selected service
        var parameters = new Dictionary<string, object>
        {
            ["Service"] = service
        };
        await _navigation.NavigateToAsync<CharacteristicsPage>(parameters);
    }

    /// <summary>
    ///     Handles connection state changes.
    /// </summary>
    private void OnConnectionStateChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(IsConnected));
        UpdateCommands();
    }

    /// <summary>
    ///     Handles service list changes.
    /// </summary>
    private void OnServiceListChanged(object? sender, ServiceListChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            var newList = Device?.GetServices() ?? [];
            Services.UpdateFrom([.. newList]);
            OnPropertyChanged(nameof(ServiceCount));
        });
    }

    /// <summary>
    ///     Updates the command can-execute states.
    /// </summary>
    private void UpdateCommands()
    {
        ConnectCommand.NotifyCanExecuteChanged();
        DisconnectCommand.NotifyCanExecuteChanged();
        ExploreServicesCommand.NotifyCanExecuteChanged();
    }
}
