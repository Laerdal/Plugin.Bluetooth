# MVVM Integration Best Practices

Integrating Plugin.Bluetooth with MVVM pattern provides clean separation of concerns, testability, and data binding support. This guide covers ViewModel patterns, data binding, and integration with MVVM frameworks.

## Table of Contents

- [INotifyPropertyChanged Support](#inotifypropertychanged-support)
- [ViewModel Design Patterns](#viewmodel-design-patterns)
- [Data Binding](#data-binding)
- [CommunityToolkit.Mvvm Integration](#communitytoolkitmvvm-integration)
- [Command Patterns](#command-patterns)
- [ObservableCollection Management](#observablecollection-management)
- [Dependency Injection](#dependency-injection)
- [Testing ViewModels](#testing-viewmodels)

## INotifyPropertyChanged Support

Plugin.Bluetooth interfaces implement `INotifyPropertyChanged` for seamless data binding.

### Understanding Property Change Notifications

```csharp
// Plugin.Bluetooth interfaces support INotifyPropertyChanged
public interface IBluetoothRemoteDevice : INotifyPropertyChanged, IAsyncDisposable
{
    string? Name { get; }
    bool IsConnected { get; }
    int SignalStrengthDbm { get; }
    // ... other properties
}

public interface IBluetoothRemoteCharacteristic : INotifyPropertyChanged, IAsyncDisposable
{
    ReadOnlyMemory<byte> Value { get; }
    bool IsListening { get; }
    // ... other properties
}
```

### Binding to Device Properties

```csharp
public class DeviceMonitorViewModel : ObservableObject
{
    private IBluetoothRemoteDevice? _device;

    public IBluetoothRemoteDevice? Device
    {
        get => _device;
        set
        {
            if (_device != null)
            {
                // Unsubscribe from old device
                _device.PropertyChanged -= OnDevicePropertyChanged;
            }

            SetProperty(ref _device, value);

            if (_device != null)
            {
                // Subscribe to new device
                _device.PropertyChanged += OnDevicePropertyChanged;

                // Raise changes for computed properties
                OnPropertyChanged(nameof(IsConnected));
                OnPropertyChanged(nameof(SignalStrength));
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }
    }

    // Bindable properties that expose device state
    public bool IsConnected => Device?.IsConnected ?? false;

    public int SignalStrength => Device?.SignalStrengthDbm ?? 0;

    public string ConnectionStatus => IsConnected ? "Connected" : "Disconnected";

    private void OnDevicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Forward specific property changes
        switch (e.PropertyName)
        {
            case nameof(IBluetoothRemoteDevice.IsConnected):
                OnPropertyChanged(nameof(IsConnected));
                OnPropertyChanged(nameof(ConnectionStatus));
                break;

            case nameof(IBluetoothRemoteDevice.SignalStrengthDbm):
                OnPropertyChanged(nameof(SignalStrength));
                break;

            case nameof(IBluetoothRemoteDevice.Name):
                // Device name changed
                break;
        }
    }
}
```

### XAML Binding

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MyApp.DeviceMonitorPage">
    <StackLayout Padding="20">
        <!-- Direct binding to ViewModel properties -->
        <Label Text="{Binding Device.Name}"
               FontSize="Large"
               FontAttributes="Bold" />

        <Label Text="{Binding ConnectionStatus}"
               TextColor="{Binding IsConnected, Converter={StaticResource BoolToColorConverter}}" />

        <Label Text="{Binding SignalStrength, StringFormat='Signal: {0} dBm'}" />

        <!-- Binding with value converter -->
        <ProgressBar Progress="{Binding SignalStrength, Converter={StaticResource RssiToProgressConverter}}"
                     ProgressColor="Green" />
    </StackLayout>
</ContentPage>
```

## ViewModel Design Patterns

### Scanner ViewModel Pattern

```csharp
public partial class ScannerViewModel : ObservableObject
{
    private readonly IBluetoothScanner _scanner;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private string _statusMessage = "Ready to scan";

    public ObservableCollection<DeviceViewModel> Devices { get; } = new();

    public ScannerViewModel(IBluetoothScanner scanner)
    {
        _scanner = scanner;

        // Subscribe to scanner events
        _scanner.RunningStateChanged += OnScanningStateChanged;
        _scanner.DeviceListChanged += OnDeviceListChanged;
    }

    [RelayCommand]
    private async Task StartScanningAsync(CancellationToken cancellationToken)
    {
        try
        {
            Devices.Clear();
            StatusMessage = "Scanning...";

            var options = new ScanningOptions
            {
                ScanMode = BluetoothScanMode.Balanced,
                IgnoreNamelessAdvertisements = true
            };

            await _scanner.StartScanningAsync(options, cancellationToken: cancellationToken);
        }
        catch (BluetoothPermissionException)
        {
            StatusMessage = "Bluetooth permission denied";
            await Shell.Current.DisplayAlert(
                "Permission Required",
                "Please grant Bluetooth permission to scan for devices.",
                "OK");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task StopScanningAsync()
    {
        try
        {
            await _scanner.StopScanningAsync();
            StatusMessage = $"Found {Devices.Count} device(s)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error stopping scan: {ex.Message}";
        }
    }

    private void OnScanningStateChanged(object? sender, EventArgs e)
    {
        IsScanning = _scanner.IsRunning;

        if (!IsScanning && Devices.Count == 0)
        {
            StatusMessage = "No devices found";
        }
    }

    private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
    {
        // Update on UI thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var device in e.AddedDevices)
            {
                var vm = new DeviceViewModel(device);
                Devices.Add(vm);
            }

            foreach (var device in e.RemovedDevices)
            {
                var vm = Devices.FirstOrDefault(d => d.Id == device.Id);
                if (vm != null)
                {
                    Devices.Remove(vm);
                }
            }

            StatusMessage = $"Found {Devices.Count} device(s)";
        });
    }
}
```

### Device ViewModel Pattern

```csharp
public partial class DeviceViewModel : ObservableObject
{
    private readonly IBluetoothRemoteDevice _device;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private int _signalStrength;

    [ObservableProperty]
    private string _connectionStatus = "Disconnected";

    public Guid Id => _device.Id;
    public string Name => _device.Name ?? "Unknown Device";

    public DeviceViewModel(IBluetoothRemoteDevice device)
    {
        _device = device;

        // Subscribe to device events
        _device.PropertyChanged += OnDevicePropertyChanged;
        _device.Connected += OnConnected;
        _device.Disconnected += OnDisconnected;
        _device.ConnectionStateChanged += OnConnectionStateChanged;

        // Initialize state
        UpdateFromDevice();
    }

    [RelayCommand]
    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            ConnectionStatus = "Connecting...";

            var options = new ConnectionOptions
            {
                ConnectionRetry = RetryOptions.Default
            };

            await _device.ConnectAsync(options, cancellationToken: cancellationToken);

            // Discover services after connection
            await _device.ExploreServicesAsync(
                ServiceExplorationOptions.WithCharacteristics,
                cancellationToken: cancellationToken);

            ConnectionStatus = "Connected";
        }
        catch (TimeoutException)
        {
            ConnectionStatus = "Connection timeout";
            await Shell.Current.DisplayAlert("Error", "Connection timed out", "OK");
        }
        catch (Exception ex)
        {
            ConnectionStatus = "Connection failed";
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task DisconnectAsync()
    {
        try
        {
            ConnectionStatus = "Disconnecting...";
            await _device.DisconnectAsync();
            ConnectionStatus = "Disconnected";
        }
        catch (Exception ex)
        {
            ConnectionStatus = "Disconnect failed";
        }
    }

    private void OnDevicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(IBluetoothRemoteDevice.IsConnected):
                    IsConnected = _device.IsConnected;
                    break;

                case nameof(IBluetoothRemoteDevice.SignalStrengthDbm):
                    SignalStrength = _device.SignalStrengthDbm;
                    break;
            }
        });
    }

    private void OnConnected(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ConnectionStatus = "Connected";
        });
    }

    private void OnDisconnected(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ConnectionStatus = "Disconnected";
        });
    }

    private void OnConnectionStateChanged(object? sender, DeviceConnectionStateChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ConnectionStatus = e.NewState.ToString();
        });
    }

    private void UpdateFromDevice()
    {
        IsConnected = _device.IsConnected;
        SignalStrength = _device.SignalStrengthDbm;
        ConnectionStatus = IsConnected ? "Connected" : "Disconnected";
    }
}
```

### Characteristic ViewModel Pattern

```csharp
public partial class CharacteristicViewModel : ObservableObject
{
    private readonly IBluetoothRemoteCharacteristic _characteristic;

    [ObservableProperty]
    private string _value = string.Empty;

    [ObservableProperty]
    private bool _isListening;

    [ObservableProperty]
    private DateTime _lastUpdated;

    public Guid Id => _characteristic.Id;
    public string Name => _characteristic.Name;
    public bool CanRead => _characteristic.CanRead;
    public bool CanWrite => _characteristic.CanWrite;
    public bool CanNotify => _characteristic.CanNotify;

    public CharacteristicViewModel(IBluetoothRemoteCharacteristic characteristic)
    {
        _characteristic = characteristic;

        // Subscribe to value changes
        _characteristic.PropertyChanged += OnCharacteristicPropertyChanged;
        _characteristic.ValueUpdated += OnValueUpdated;

        // Initialize
        UpdateValue();
    }

    [RelayCommand(CanExecute = nameof(CanRead))]
    private async Task ReadValueAsync()
    {
        try
        {
            var value = await _characteristic.ReadValueAsync();
            UpdateValue();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to read: {ex.Message}", "OK");
        }
    }

    [RelayCommand(CanExecute = nameof(CanWrite))]
    private async Task WriteValueAsync(string valueText)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(valueText);
            await _characteristic.WriteValueAsync(bytes);
            UpdateValue();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to write: {ex.Message}", "OK");
        }
    }

    [RelayCommand(CanExecute = nameof(CanNotify))]
    private async Task ToggleNotificationsAsync()
    {
        try
        {
            if (IsListening)
            {
                await _characteristic.StopListeningAsync();
            }
            else
            {
                await _characteristic.StartListeningAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to toggle notifications: {ex.Message}", "OK");
        }
    }

    private void OnCharacteristicPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(IBluetoothRemoteCharacteristic.Value):
                    UpdateValue();
                    break;

                case nameof(IBluetoothRemoteCharacteristic.IsListening):
                    IsListening = _characteristic.IsListening;
                    break;
            }
        });
    }

    private void OnValueUpdated(object? sender, CharacteristicValueUpdatedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateValue();
            LastUpdated = DateTime.Now;
        });
    }

    private void UpdateValue()
    {
        var bytes = _characteristic.Value.ToArray();
        Value = BitConverter.ToString(bytes).Replace("-", " ");
    }
}
```

## Data Binding

### Value Converters

```csharp
// RSSI to Progress Converter (for signal strength visualization)
public class RssiToProgressConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int rssi)
        {
            // RSSI typically ranges from -100 (weakest) to -30 (strongest)
            // Convert to 0.0 - 1.0 range
            var normalized = (rssi + 100) / 70.0;
            return Math.Max(0, Math.Min(1, normalized));
        }

        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Bool to Color Converter (for connection status)
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isConnected)
        {
            return isConnected ? Colors.Green : Colors.Red;
        }

        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Byte Array to String Converter
public class ByteArrayToHexStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ReadOnlyMemory<byte> memory)
        {
            var bytes = memory.ToArray();
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        if (value is byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hex)
        {
            return System.Convert.FromHexString(hex.Replace(" ", ""));
        }

        return Array.Empty<byte>();
    }
}
```

### Data Templates

```xml
<!-- DeviceTemplate.xaml -->
<DataTemplate xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
              xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
              xmlns:vm="clr-namespace:MyApp.ViewModels"
              x:DataType="vm:DeviceViewModel">
    <SwipeView>
        <SwipeView.RightItems>
            <SwipeItems>
                <SwipeItem Text="Connect"
                           BackgroundColor="Green"
                           Command="{Binding ConnectCommand}"
                           IsVisible="{Binding IsConnected, Converter={StaticResource InvertBoolConverter}}" />
                <SwipeItem Text="Disconnect"
                           BackgroundColor="Red"
                           Command="{Binding DisconnectCommand}"
                           IsVisible="{Binding IsConnected}" />
            </SwipeItems>
        </SwipeView.RightItems>

        <Grid Padding="10">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
            </Grid.RowDefinitions>

            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>

            <Label Grid.Row="0" Grid.Column="0"
                   Text="{Binding Name}"
                   FontSize="Medium"
                   FontAttributes="Bold" />

            <Label Grid.Row="1" Grid.Column="0"
                   Text="{Binding ConnectionStatus}"
                   FontSize="Small"
                   TextColor="{Binding IsConnected, Converter={StaticResource BoolToColorConverter}}" />

            <Label Grid.Row="0" Grid.Column="1"
                   Grid.RowSpan="2"
                   Text="{Binding SignalStrength, StringFormat='{0} dBm'}"
                   VerticalOptions="Center"
                   FontSize="Small" />
        </Grid>
    </SwipeView>
</DataTemplate>
```

## CommunityToolkit.Mvvm Integration

### Source Generators for Clean ViewModels

```csharp
// Using CommunityToolkit.Mvvm source generators
public partial class ModernDeviceViewModel : ObservableObject
{
    private readonly IBluetoothRemoteDevice _device;

    // Properties with automatic change notification
    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private int _signalStrength;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotConnecting))]
    private bool _isConnecting;

    public bool IsNotConnecting => !IsConnecting;

    public ModernDeviceViewModel(IBluetoothRemoteDevice device)
    {
        _device = device;
        _device.PropertyChanged += OnDevicePropertyChanged;
    }

    // Async command with cancellation support
    [RelayCommand]
    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        IsConnecting = true;

        try
        {
            await _device.ConnectAsync(cancellationToken: cancellationToken);
            await _device.ExploreServicesAsync(
                ServiceExplorationOptions.WithCharacteristics,
                cancellationToken: cancellationToken);
        }
        finally
        {
            IsConnecting = false;
        }
    }

    // Command with CanExecute
    [RelayCommand(CanExecute = nameof(IsConnected))]
    private async Task DisconnectAsync()
    {
        await _device.DisconnectAsync();
    }

    private void OnDevicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(IBluetoothRemoteDevice.IsConnected):
                    IsConnected = _device.IsConnected;
                    // Automatically updates CanExecute for DisconnectCommand
                    DisconnectCommand.NotifyCanExecuteChanged();
                    break;

                case nameof(IBluetoothRemoteDevice.SignalStrengthDbm):
                    SignalStrength = _device.SignalStrengthDbm;
                    break;
            }
        });
    }
}
```

### ObservableValidator for Input Validation

```csharp
public partial class WriteCharacteristicViewModel : ObservableValidator
{
    private readonly IBluetoothRemoteCharacteristic _characteristic;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Value is required")]
    [MinLength(1, ErrorMessage = "Value must not be empty")]
    [MaxLength(512, ErrorMessage = "Value too long (max 512 bytes)")]
    private string _inputValue = string.Empty;

    [ObservableProperty]
    private bool _isWriting;

    public WriteCharacteristicViewModel(IBluetoothRemoteCharacteristic characteristic)
    {
        _characteristic = characteristic;
    }

    [RelayCommand(CanExecute = nameof(CanWrite))]
    private async Task WriteAsync()
    {
        // Validate input
        ValidateAllProperties();

        if (HasErrors)
            return;

        IsWriting = true;

        try
        {
            var bytes = Encoding.UTF8.GetBytes(InputValue);
            await _characteristic.WriteValueAsync(bytes);

            await Shell.Current.DisplayAlert("Success", "Value written", "OK");
            InputValue = string.Empty;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsWriting = false;
        }
    }

    private bool CanWrite() => !IsWriting && !HasErrors;

    partial void OnInputValueChanged(string value)
    {
        // Trigger validation and update CanExecute
        ValidateProperty(value, nameof(InputValue));
        WriteCommand.NotifyCanExecuteChanged();
    }
}
```

## Command Patterns

### Cancellable Commands

```csharp
public partial class ScanViewModel : ObservableObject
{
    private readonly IBluetoothScanner _scanner;
    private CancellationTokenSource? _scanCts;

    [ObservableProperty]
    private bool _isScanning;

    public ScanViewModel(IBluetoothScanner scanner)
    {
        _scanner = scanner;
    }

    [RelayCommand]
    private async Task StartScanAsync()
    {
        _scanCts?.Cancel();
        _scanCts = new CancellationTokenSource();

        IsScanning = true;

        try
        {
            // Scan with timeout
            _scanCts.CancelAfter(TimeSpan.FromSeconds(30));

            await _scanner.StartScanningAsync(cancellationToken: _scanCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Scan timed out or cancelled
        }
        finally
        {
            IsScanning = false;
        }
    }

    [RelayCommand]
    private async Task StopScanAsync()
    {
        _scanCts?.Cancel();
        await _scanner.StopScanningAsync();
        IsScanning = false;
    }
}
```

### Progress Reporting Commands

```csharp
public partial class DataTransferViewModel : ObservableObject
{
    private readonly IBluetoothRemoteCharacteristic _characteristic;

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    private async Task TransferDataAsync(byte[] data)
    {
        var device = _characteristic.RemoteService.RemoteDevice;
        var mtu = device.Mtu;
        var payloadSize = mtu - 3;
        var totalChunks = (int)Math.Ceiling(data.Length / (double)payloadSize);

        Progress = 0;
        StatusMessage = $"Transferring {data.Length} bytes...";

        for (int i = 0; i < data.Length; i += payloadSize)
        {
            var chunkSize = Math.Min(payloadSize, data.Length - i);
            var chunk = data.AsMemory(i, chunkSize);

            await _characteristic.WriteValueAsync(chunk);

            // Update progress
            Progress = (double)(i + chunkSize) / data.Length;
            StatusMessage = $"Transferred {i + chunkSize}/{data.Length} bytes";
        }

        StatusMessage = "Transfer complete";
    }
}
```

## ObservableCollection Management

### Thread-Safe Collection Updates

```csharp
public class ThreadSafeDeviceCollection : ObservableCollection<DeviceViewModel>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task AddDeviceAsync(DeviceViewModel device)
    {
        await _semaphore.WaitAsync();

        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Add(device);
            });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task RemoveDeviceAsync(Guid deviceId)
    {
        await _semaphore.WaitAsync();

        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var device = this.FirstOrDefault(d => d.Id == deviceId);
                if (device != null)
                {
                    Remove(device);
                }
            });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ClearDevicesAsync()
    {
        await _semaphore.WaitAsync();

        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Clear();
            });
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

### Sorted and Filtered Collections

```csharp
public partial class AdvancedScannerViewModel : ObservableObject
{
    private readonly IBluetoothScanner _scanner;

    public ObservableCollection<DeviceViewModel> AllDevices { get; } = new();

    // Filtered collection for display
    public ObservableCollection<DeviceViewModel> FilteredDevices { get; } = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _showOnlyConnectedDevices;

    partial void OnSearchTextChanged(string value)
    {
        UpdateFilter();
    }

    partial void OnShowOnlyConnectedDevicesChanged(bool value)
    {
        UpdateFilter();
    }

    private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var device in e.AddedDevices)
            {
                var vm = new DeviceViewModel(device);
                AllDevices.Add(vm);
            }

            foreach (var device in e.RemovedDevices)
            {
                var vm = AllDevices.FirstOrDefault(d => d.Id == device.Id);
                if (vm != null)
                {
                    AllDevices.Remove(vm);
                }
            }

            UpdateFilter();
        });
    }

    private void UpdateFilter()
    {
        FilteredDevices.Clear();

        var query = AllDevices.AsEnumerable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(d =>
                d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        // Apply connection filter
        if (ShowOnlyConnectedDevices)
        {
            query = query.Where(d => d.IsConnected);
        }

        // Sort by signal strength (strongest first)
        query = query.OrderByDescending(d => d.SignalStrength);

        foreach (var device in query)
        {
            FilteredDevices.Add(device);
        }
    }
}
```

## Dependency Injection

### Registering ViewModels

```csharp
// MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit();

        // Register Bluetooth services
        builder.Services.AddBluetoothServices();

        // Register ViewModels
        builder.Services.AddTransient<ScannerViewModel>();
        builder.Services.AddTransient<DeviceViewModel>();
        builder.Services.AddTransient<CharacteristicViewModel>();

        // Register Pages
        builder.Services.AddTransient<ScannerPage>();
        builder.Services.AddTransient<DevicePage>();

        return builder.Build();
    }
}
```

### ViewModel with Multiple Dependencies

```csharp
public partial class AdvancedBluetoothViewModel : ObservableObject
{
    private readonly IBluetoothScanner _scanner;
    private readonly ILogger<AdvancedBluetoothViewModel> _logger;
    private readonly IConnectivity _connectivity;

    public AdvancedBluetoothViewModel(
        IBluetoothScanner scanner,
        ILogger<AdvancedBluetoothViewModel> logger,
        IConnectivity connectivity)
    {
        _scanner = scanner;
        _logger = logger;
        _connectivity = connectivity;

        _connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        _logger.LogInformation("Network connectivity changed: {State}", e.NetworkAccess);

        // React to connectivity changes if needed
    }
}
```

## Testing ViewModels

### Mocking Bluetooth Interfaces

```csharp
public class ScannerViewModelTests
{
    [Fact]
    public async Task StartScanning_ShouldUpdateIsScanning()
    {
        // Arrange
        var mockScanner = new Mock<IBluetoothScanner>();
        mockScanner.Setup(s => s.StartScanningAsync(
            It.IsAny<ScanningOptions>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var viewModel = new ScannerViewModel(mockScanner.Object);

        // Act
        await viewModel.StartScanningCommand.ExecuteAsync(null);

        // Assert
        mockScanner.Verify(s => s.StartScanningAsync(
            It.IsAny<ScanningOptions>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void OnDeviceListChanged_ShouldAddDevicesToCollection()
    {
        // Arrange
        var mockScanner = new Mock<IBluetoothScanner>();
        var mockDevice = new Mock<IBluetoothRemoteDevice>();
        mockDevice.Setup(d => d.Id).Returns(Guid.NewGuid());
        mockDevice.Setup(d => d.Name).Returns("Test Device");

        var viewModel = new ScannerViewModel(mockScanner.Object);

        // Act
        var args = new DeviceListChangedEventArgs
        {
            AddedDevices = new[] { mockDevice.Object },
            RemovedDevices = Array.Empty<IBluetoothRemoteDevice>()
        };

        mockScanner.Raise(s => s.DeviceListChanged += null, mockScanner.Object, args);

        // Assert
        Assert.Single(viewModel.Devices);
        Assert.Equal("Test Device", viewModel.Devices[0].Name);
    }
}
```

## Summary

### MVVM Integration Checklist

- [ ] Use **INotifyPropertyChanged** support from Plugin.Bluetooth interfaces
- [ ] Create **dedicated ViewModels** for devices, characteristics, and services
- [ ] Use **ObservableObject** base class (CommunityToolkit.Mvvm)
- [ ] Implement **RelayCommand** for async operations with cancellation
- [ ] Update UI on **MainThread** for cross-thread property changes
- [ ] Use **value converters** for data presentation
- [ ] Implement **data templates** for reusable UI components
- [ ] Register ViewModels and services with **dependency injection**
- [ ] Use **ObservableCollection** for device/characteristic lists
- [ ] Implement **filtering and sorting** for better UX
- [ ] Subscribe to **property changes and events** properly
- [ ] Unsubscribe from events in **cleanup/dispose**
- [ ] Write **unit tests** with mocked interfaces

### Related Topics

- [Testing](Testing.md) - Testing strategies for BLE ViewModels
- [Error Handling](Error-Handling.md) - Handling errors in ViewModels
- [Connection Management](Connection-Management.md) - Managing connections in ViewModels
