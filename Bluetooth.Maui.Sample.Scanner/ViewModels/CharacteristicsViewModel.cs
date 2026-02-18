using Plugin.BaseTypeExtensions;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
/// ViewModel for the characteristics page, handling service details and characteristic exploration.
/// </summary>
public class CharacteristicsViewModel : BaseViewModel
{
    private readonly INavigationService _navigation;
    private IBluetoothRemoteService? _service;

    /// <summary>
    /// Gets or sets the current Bluetooth service.
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
    /// Gets the service ID for display.
    /// </summary>
    public string ServiceId => Service?.Id.ToString() ?? "N/A";

    /// <summary>
    /// Gets the collection of discovered characteristics.
    /// </summary>
    public ObservableCollection<IBluetoothRemoteCharacteristic> Characteristics { get; } = new();

    /// <summary>
    /// Gets the number of discovered characteristics.
    /// </summary>
    public int CharacteristicCount => Characteristics.Count;

    /// <summary>
    /// Gets the command to explore characteristics on the service.
    /// </summary>
    public IAsyncRelayCommand ExploreCharacteristicsCommand { get; }

    /// <summary>
    /// Gets the command to select and interact with a characteristic.
    /// </summary>
    public IAsyncRelayCommand<IBluetoothRemoteCharacteristic> SelectCharacteristicCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicsViewModel"/> class.
    /// </summary>
    public CharacteristicsViewModel(INavigationService navigation)
    {
        _navigation = navigation;

        ExploreCharacteristicsCommand = new AsyncRelayCommand(ExploreCharacteristicsAsync);
        SelectCharacteristicCommand = new AsyncRelayCommand<IBluetoothRemoteCharacteristic>(SelectCharacteristicAsync);
    }

    /// <summary>
    /// Sets the service to display and manage.
    /// </summary>
    /// <param name="service">The Bluetooth service.</param>
    public void SetService(IBluetoothRemoteService service)
    {
        Service = service;
    }

    /// <summary>
    /// Explores and discovers characteristics on the service.
    /// </summary>
    private async Task ExploreCharacteristicsAsync()
    {
        if (Service == null) return;

        try
        {
            // Explore characteristics on the service
            await Service.ExploreCharacteristicsAsync();

            // Update the characteristics collection on the UI thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Characteristics.UpdateFrom(Service.GetCharacteristics());
                OnPropertyChanged(nameof(CharacteristicCount));
            });
        }
        catch (Exception ex)
        {
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
    /// Handles characteristic selection.
    /// </summary>
    private async Task SelectCharacteristicAsync(IBluetoothRemoteCharacteristic? characteristic)
    {
        if (characteristic == null) return;

        // Display characteristic details
        var properties = new List<string>();
        if (characteristic.CanRead) properties.Add("Read");
        if (characteristic.CanWrite) properties.Add("Write");
        if (characteristic.CanListen) properties.Add("Listen/Notify");

        var propertiesText = properties.Any()
            ? string.Join(", ", properties)
            : "None";

        var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (mainPage != null)
        {
            await mainPage.DisplayAlertAsync(
                "Characteristic Details",
                $"UUID: {characteristic.Id}\n\nProperties: {propertiesText}",
                "OK");
        }
    }

    /// <summary>
    /// Handles characteristic list changes.
    /// </summary>
    private void OnCharacteristicListChanged(object? sender, CharacteristicListChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Characteristics.UpdateFrom(Service?.GetCharacteristics() ?? Enumerable.Empty<IBluetoothRemoteCharacteristic>());
            OnPropertyChanged(nameof(CharacteristicCount));
        });
    }
}
