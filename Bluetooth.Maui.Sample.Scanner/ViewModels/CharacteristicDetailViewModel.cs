using Microsoft.Extensions.Logging;
using System.Text;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for the characteristic detail page, handling read/write/notify operations.
/// </summary>
public class CharacteristicDetailViewModel : BaseViewModel
{
    private readonly ILogger<CharacteristicDetailViewModel> _logger;
    private IBluetoothRemoteCharacteristic? _characteristic;
    private bool _isHexMode = true;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicDetailViewModel" /> class.
    /// </summary>
    public CharacteristicDetailViewModel(ILogger<CharacteristicDetailViewModel> logger)
    {
        _logger = logger;

        ReadValueCommand = new AsyncRelayCommand(ReadValueAsync, () => Characteristic?.CanRead ?? false);
        WriteValueCommand = new AsyncRelayCommand(WriteValueAsync, () => Characteristic?.CanWrite ?? false);
        ToggleListenCommand = new AsyncRelayCommand(ToggleListenAsync, () => Characteristic?.CanListen ?? false);
        ToggleDisplayModeCommand = new RelayCommand(ToggleDisplayMode);
    }

    /// <summary>
    ///     Gets or sets the current Bluetooth characteristic.
    /// </summary>
    public IBluetoothRemoteCharacteristic? Characteristic
    {
        get => _characteristic;
        private set
        {
            if (_characteristic != null)
            {
                _characteristic.ValueUpdated -= OnValueUpdated;
            }

            _characteristic = value;

            if (_characteristic != null)
            {
                _characteristic.ValueUpdated += OnValueUpdated;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(CharacteristicId));
            OnPropertyChanged(nameof(CanRead));
            OnPropertyChanged(nameof(CanWrite));
            OnPropertyChanged(nameof(CanListen));
            OnPropertyChanged(nameof(IsListening));
            UpdateCommands();
        }
    }

    /// <summary>
    ///     Gets the characteristic ID for display.
    /// </summary>
    public string CharacteristicId => Characteristic?.Id.ToString() ?? "N/A";

    /// <summary>
    ///     Gets whether the characteristic supports reading.
    /// </summary>
    public bool CanRead => Characteristic?.CanRead ?? false;

    /// <summary>
    ///     Gets whether the characteristic supports writing.
    /// </summary>
    public bool CanWrite => Characteristic?.CanWrite ?? false;

    /// <summary>
    ///     Gets whether the characteristic supports notifications/indications.
    /// </summary>
    public bool CanListen => Characteristic?.CanListen ?? false;

    /// <summary>
    ///     Gets whether we're currently listening for notifications.
    /// </summary>
    public bool IsListening => Characteristic?.IsListening ?? false;

    /// <summary>
    ///     Gets or sets the current value display.
    /// </summary>
    public string CurrentValue
    {
        get => GetValue(string.Empty);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets the input value for writing.
    /// </summary>
    public string WriteValueInput
    {
        get => GetValue(string.Empty);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets whether to display values in hex mode.
    /// </summary>
    public bool IsHexMode
    {
        get => _isHexMode;
        set
        {
            _isHexMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayModeText));
        }
    }

    /// <summary>
    ///     Gets the display mode text.
    /// </summary>
    public string DisplayModeText => IsHexMode ? "HEX" : "TEXT";

    /// <summary>
    ///     Gets the command to read the characteristic value.
    /// </summary>
    public IAsyncRelayCommand ReadValueCommand { get; }

    /// <summary>
    ///     Gets the command to write a value to the characteristic.
    /// </summary>
    public IAsyncRelayCommand WriteValueCommand { get; }

    /// <summary>
    ///     Gets the command to toggle notification subscription.
    /// </summary>
    public IAsyncRelayCommand ToggleListenCommand { get; }

    /// <summary>
    ///     Gets the command to toggle between hex and text display modes.
    /// </summary>
    public IRelayCommand ToggleDisplayModeCommand { get; }

    /// <summary>
    ///     Sets the characteristic to interact with.
    /// </summary>
    public void SetCharacteristic(IBluetoothRemoteCharacteristic characteristic)
    {
        Characteristic = characteristic;
        _logger.LogInformation("Characteristic detail view initialized for {CharacteristicId}", characteristic.Id);

        // Try to read the initial value if readable
        if (characteristic.CanRead)
        {
            _ = ReadValueAsync();
        }
    }

    /// <summary>
    ///     Reads the current value from the characteristic.
    /// </summary>
    private async Task ReadValueAsync()
    {
        if (Characteristic == null)
        {
            return;
        }

        try
        {
            _logger.LogInformation("Reading value from characteristic {CharacteristicId}", Characteristic.Id);
            var value = await Characteristic.ReadValueAsync();

            CurrentValue = FormatBytes(value.ToArray());
            _logger.LogInformation("Successfully read {Length} bytes from characteristic {CharacteristicId}",
                value.Length, Characteristic.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read value from characteristic {CharacteristicId}", Characteristic.Id);
            CurrentValue = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    ///     Writes a value to the characteristic.
    /// </summary>
    private async Task WriteValueAsync()
    {
        if (Characteristic == null || string.IsNullOrWhiteSpace(WriteValueInput))
        {
            return;
        }

        try
        {
            byte[] data;

            if (IsHexMode)
            {
                // Parse hex string
                var hexString = WriteValueInput.Replace(" ", "").Replace("0x", "");
                data = Enumerable.Range(0, hexString.Length / 2)
                    .Select(x => Convert.ToByte(hexString.Substring(x * 2, 2), 16))
                    .ToArray();
            }
            else
            {
                // Convert text to bytes
                data = Encoding.UTF8.GetBytes(WriteValueInput);
            }

            _logger.LogInformation("Writing {Length} bytes to characteristic {CharacteristicId}",
                data.Length, Characteristic.Id);

            await Characteristic.WriteValueAsync(data);

            _logger.LogInformation("Successfully wrote {Length} bytes to characteristic {CharacteristicId}",
                data.Length, Characteristic.Id);

            WriteValueInput = string.Empty;
            CurrentValue = $"✓ Wrote: {FormatBytes(data)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write value to characteristic {CharacteristicId}", Characteristic.Id);
            CurrentValue = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    ///     Toggles notification subscription.
    /// </summary>
    private async Task ToggleListenAsync()
    {
        if (Characteristic == null)
        {
            return;
        }

        try
        {
            if (Characteristic.IsListening)
            {
                _logger.LogInformation("Stopping notifications for characteristic {CharacteristicId}", Characteristic.Id);
                await Characteristic.StopListeningAsync();
                _logger.LogInformation("Successfully stopped notifications for characteristic {CharacteristicId}",
                    Characteristic.Id);
            }
            else
            {
                _logger.LogInformation("Starting notifications for characteristic {CharacteristicId}", Characteristic.Id);
                await Characteristic.StartListeningAsync();
                _logger.LogInformation("Successfully started notifications for characteristic {CharacteristicId}",
                    Characteristic.Id);
            }

            OnPropertyChanged(nameof(IsListening));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle notifications for characteristic {CharacteristicId}", Characteristic.Id);
            CurrentValue = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    ///     Toggles between hex and text display modes.
    /// </summary>
    private void ToggleDisplayMode()
    {
        IsHexMode = !IsHexMode;
        _logger.LogDebug("Display mode toggled to: {Mode}", IsHexMode ? "HEX" : "TEXT");
    }

    /// <summary>
    ///     Formats bytes for display based on current mode.
    /// </summary>
    private string FormatBytes(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return "(empty)";
        }

        if (IsHexMode)
        {
            return "0x" + BitConverter.ToString(bytes).Replace("-", " ");
        }
        else
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }

    /// <summary>
    ///     Handles value update events from the characteristic.
    /// </summary>
    private void OnValueUpdated(object? sender, ValueUpdatedEventArgs e)
    {
        _logger.LogDebug("Characteristic {CharacteristicId} value updated, {Length} bytes",
            Characteristic?.Id, e.NewValue.Length);

        MainThread.BeginInvokeOnMainThread(() => {
            CurrentValue = $"📬 {FormatBytes(e.NewValue.ToArray())}";
        });
    }

    /// <summary>
    ///     Updates all command can-execute states.
    /// </summary>
    private void UpdateCommands()
    {
        ReadValueCommand.NotifyCanExecuteChanged();
        WriteValueCommand.NotifyCanExecuteChanged();
        ToggleListenCommand.NotifyCanExecuteChanged();
    }
}
