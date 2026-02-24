using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for the characteristics page, handling service details and characteristic exploration.
/// </summary>
public class CharacteristicsViewModel : BaseViewModel
{
    private readonly ILogger<CharacteristicsViewModel> _logger;
    private readonly INavigationService _navigation;
    private IBluetoothRemoteService? _service;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicsViewModel" /> class.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="logger">The logger instance.</param>
    public CharacteristicsViewModel(INavigationService navigation, ILogger<CharacteristicsViewModel> logger)
    {
        _navigation = navigation;
        _logger = logger;

        ExploreCharacteristicsCommand = new AsyncRelayCommand(ExploreCharacteristicsAsync);
        SelectCharacteristicCommand = new AsyncRelayCommand<IBluetoothRemoteCharacteristic>(SelectCharacteristicAsync);
    }

    /// <summary>
    ///     Gets or sets the current Bluetooth service.
    /// </summary>
    public IBluetoothRemoteService? Service
    {
        get => _service;
        private set
        {
            if (_service != null)
            {
                _service.CharacteristicListChanged -= OnCharacteristicListChanged;
            }

            _service = value;

            if (_service != null)
            {
                _service.CharacteristicListChanged += OnCharacteristicListChanged;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(ServiceId));
            OnPropertyChanged(nameof(CharacteristicCount));
        }
    }

    /// <summary>
    ///     Gets the service ID for display.
    /// </summary>
    public string ServiceId => Service?.Id.ToString() ?? "N/A";

    /// <summary>
    ///     Gets the collection of discovered characteristics.
    /// </summary>
    public ObservableCollection<IBluetoothRemoteCharacteristic> Characteristics { get; } = new ObservableCollection<IBluetoothRemoteCharacteristic>();

    /// <summary>
    ///     Gets the number of discovered characteristics.
    /// </summary>
    public int CharacteristicCount => Characteristics.Count;

    /// <summary>
    ///     Gets the command to explore characteristics on the service.
    /// </summary>
    public IAsyncRelayCommand ExploreCharacteristicsCommand { get; }

    /// <summary>
    ///     Gets the command to select and interact with a characteristic.
    /// </summary>
    public IAsyncRelayCommand<IBluetoothRemoteCharacteristic> SelectCharacteristicCommand { get; }

    /// <summary>
    ///     Sets the service to display and manage.
    /// </summary>
    /// <param name="service">The Bluetooth service.</param>
    public void SetService(IBluetoothRemoteService service)
    {
        Service = service;
    }

    /// <summary>
    ///     Explores and discovers characteristics on the service.
    /// </summary>
    private async Task ExploreCharacteristicsAsync()
    {
        if (Service == null)
        {
            return;
        }

        try
        {
            _logger.LogInformation("Exploring characteristics for service: {ServiceId}", ServiceId);

            // Explore characteristics on the service
            await Service.ExploreCharacteristicsAsync();

            // Update the characteristics collection on the UI thread
            MainThread.BeginInvokeOnMainThread(() => {
                Characteristics.UpdateFrom([.. Service.GetCharacteristics()]);
                OnPropertyChanged(nameof(CharacteristicCount));
            });

            _logger.LogInformation("Characteristic exploration completed - Found {CharacteristicCount} characteristics for service: {ServiceId}", CharacteristicCount, ServiceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to explore characteristics for service: {ServiceId}", ServiceId);

            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlertAsync(
                    "Characteristic Discovery Error",
                    $"Failed to discover characteristics: {ex.Message}",
                    "OK");
            }
        }
    }

    /// <summary>
    ///     Handles characteristic selection and navigates to the detail page.
    /// </summary>
    private async Task SelectCharacteristicAsync(IBluetoothRemoteCharacteristic? characteristic)
    {
        if (characteristic == null)
        {
            return;
        }

        _logger.LogInformation("Characteristic selected: {CharacteristicId} from service: {ServiceId}", characteristic.Id, ServiceId);

        // Navigate to the characteristic detail page with the selected characteristic
        var parameters = new Dictionary<string, object>
        {
            ["Characteristic"] = characteristic
        };
        await _navigation.NavigateToAsync<CharacteristicDetailPage>(parameters);
    }

    /// <summary>
    ///     Handles characteristic list changes.
    /// </summary>
    private void OnCharacteristicListChanged(object? sender, CharacteristicListChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            var newList = Service?.GetCharacteristics() ?? [];
            Characteristics.UpdateFrom([.. newList]);
            OnPropertyChanged(nameof(CharacteristicCount));
        });
    }
}
