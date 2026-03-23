using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Sample.Scanner.ViewModels;

/// <summary>
///     ViewModel for the characteristic write/listen lab page.
/// </summary>
public class WriteListenLabViewModel : BaseViewModel
{
    private readonly ILogger<WriteListenLabViewModel> _logger;

    private IBluetoothRemoteCharacteristic? _characteristic;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WriteListenLabViewModel" /> class.
    /// </summary>
    public WriteListenLabViewModel(ILogger<WriteListenLabViewModel> logger)
    {
        _logger = logger;

        IsHexMode = true;
        WriteValueInput = "01 02 03";

        ReadValueCommand = new AsyncRelayCommand(ReadValueAsync, () => Characteristic?.CanRead == true);
        WriteValueCommand = new AsyncRelayCommand(WriteValueAsync, () => Characteristic?.CanWrite == true);
        ToggleListenCommand = new AsyncRelayCommand(ToggleListenAsync, () => Characteristic?.CanListen == true);
        ToggleDisplayModeCommand = new RelayCommand(ToggleDisplayMode);
        WritePingCommand = new AsyncRelayCommand(() => QuickWriteAsync("PING"), () => Characteristic?.CanWrite == true);
        WriteHelloCommand = new AsyncRelayCommand(() => QuickWriteAsync("HELLO"), () => Characteristic?.CanWrite == true);
    }

    /// <summary>
    ///     Gets or sets the characteristic under test.
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
    ///     Gets the characteristic id as string.
    /// </summary>
    public string CharacteristicId => Characteristic?.Id.ToString() ?? "N/A";

    /// <summary>
    ///     Gets whether current characteristic can be read.
    /// </summary>
    public bool CanRead => Characteristic?.CanRead == true;

    /// <summary>
    ///     Gets whether current characteristic can be written.
    /// </summary>
    public bool CanWrite => Characteristic?.CanWrite == true;

    /// <summary>
    ///     Gets whether current characteristic can be listened to.
    /// </summary>
    public bool CanListen => Characteristic?.CanListen == true;

    /// <summary>
    ///     Gets whether current characteristic is listening.
    /// </summary>
    public bool IsListening => Characteristic?.IsListening == true;

    /// <summary>
    ///     Gets or sets the write value input.
    /// </summary>
    public string WriteValueInput
    {
        get => GetValue(string.Empty);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets whether hex mode is enabled.
    /// </summary>
    public bool IsHexMode
    {
        get => GetValue(true);
        set
        {
            if (SetValue(value))
            {
                OnPropertyChanged(nameof(DisplayModeText));
            }
        }
    }

    /// <summary>
    ///     Gets display mode text.
    /// </summary>
    public string DisplayModeText => IsHexMode ? "HEX" : "TEXT";

    /// <summary>
    ///     Gets or sets current value display text.
    /// </summary>
    public string CurrentValue
    {
        get => GetValue("(no value yet)");
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets lab activity log entries.
    /// </summary>
    public ObservableCollection<string> ActivityLog { get; } = new();

    /// <summary>
    ///     Gets read command.
    /// </summary>
    public IAsyncRelayCommand ReadValueCommand { get; }

    /// <summary>
    ///     Gets write command.
    /// </summary>
    public IAsyncRelayCommand WriteValueCommand { get; }

    /// <summary>
    ///     Gets listen toggle command.
    /// </summary>
    public IAsyncRelayCommand ToggleListenCommand { get; }

    /// <summary>
    ///     Gets display mode toggle command.
    /// </summary>
    public IRelayCommand ToggleDisplayModeCommand { get; }

    /// <summary>
    ///     Gets quick write command for PING payload.
    /// </summary>
    public IAsyncRelayCommand WritePingCommand { get; }

    /// <summary>
    ///     Gets quick write command for HELLO payload.
    /// </summary>
    public IAsyncRelayCommand WriteHelloCommand { get; }

    /// <summary>
    ///     Sets characteristic context for the lab.
    /// </summary>
    public void SetCharacteristic(IBluetoothRemoteCharacteristic characteristic)
    {
        Characteristic = characteristic;
        AppendLog($"Lab opened for characteristic {characteristic.Id}");

        if (characteristic.CanRead)
        {
            _ = ReadValueAsync();
        }
    }

    private async Task ReadValueAsync()
    {
        if (Characteristic == null)
        {
            return;
        }

        try
        {
            var value = await Characteristic.ReadValueAsync().ConfigureAwait(false);
            var text = FormatBytes(value.ToArray());
            CurrentValue = text;
            AppendLog($"Read {value.Length} bytes: {text}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lab read failed for characteristic {CharacteristicId}", Characteristic.Id);
            CurrentValue = $"Error: {ex.Message}";
            AppendLog(CurrentValue);
        }
    }

    private async Task WriteValueAsync()
    {
        if (Characteristic == null)
        {
            return;
        }

        try
        {
            var bytes = ParseInputToBytes(WriteValueInput);
            await Characteristic.WriteValueAsync(bytes).ConfigureAwait(false);
            AppendLog($"Wrote {bytes.Length} bytes: {FormatBytes(bytes)}");
            WriteValueInput = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lab write failed for characteristic {CharacteristicId}", Characteristic.Id);
            CurrentValue = $"Error: {ex.Message}";
            AppendLog(CurrentValue);
        }
    }

    private async Task QuickWriteAsync(string text)
    {
        WriteValueInput = IsHexMode
            ? BitConverter.ToString(Encoding.UTF8.GetBytes(text)).Replace("-", " ")
            : text;

        await WriteValueAsync().ConfigureAwait(false);
    }

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
                await Characteristic.StopListeningAsync().ConfigureAwait(false);
                AppendLog("Stopped listening");
            }
            else
            {
                await Characteristic.StartListeningAsync().ConfigureAwait(false);
                AppendLog("Started listening");
            }

            OnPropertyChanged(nameof(IsListening));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lab listen toggle failed for characteristic {CharacteristicId}", Characteristic.Id);
            CurrentValue = $"Error: {ex.Message}";
            AppendLog(CurrentValue);
        }
    }

    private void ToggleDisplayMode()
    {
        IsHexMode = !IsHexMode;
        AppendLog($"Display mode: {DisplayModeText}");
    }

    private void OnValueUpdated(object? sender, ValueUpdatedEventArgs e)
    {
        var newText = FormatBytes(e.NewValue.ToArray());
        MainThread.BeginInvokeOnMainThread(() => {
            CurrentValue = $"📬 {newText}";
            AppendLog($"Notification received: {newText}");
            OnPropertyChanged(nameof(IsListening));
        });
    }

    private byte[] ParseInputToBytes(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Write input cannot be empty.");
        }

        if (!IsHexMode)
        {
            return Encoding.UTF8.GetBytes(input);
        }

        var normalized = input.Replace(" ", string.Empty)
                              .Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase);

        if (normalized.Length % 2 != 0)
        {
            throw new FormatException("Hex input must contain an even number of characters.");
        }

        return Enumerable.Range(0, normalized.Length / 2)
                         .Select(index => Convert.ToByte(normalized.Substring(index * 2, 2), 16))
                         .ToArray();
    }

    private string FormatBytes(byte[] bytes)
    {
        if (bytes.Length == 0)
        {
            return "(empty)";
        }

        return IsHexMode
            ? "0x" + BitConverter.ToString(bytes).Replace("-", " ")
            : Encoding.UTF8.GetString(bytes);
    }

    private void AppendLog(string message)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
        MainThread.BeginInvokeOnMainThread(() => {
            ActivityLog.Insert(0, entry);
            while (ActivityLog.Count > 60)
            {
                ActivityLog.RemoveAt(ActivityLog.Count - 1);
            }
        });
    }

    private void UpdateCommands()
    {
        ReadValueCommand.NotifyCanExecuteChanged();
        WriteValueCommand.NotifyCanExecuteChanged();
        ToggleListenCommand.NotifyCanExecuteChanged();
        WritePingCommand.NotifyCanExecuteChanged();
        WriteHelloCommand.NotifyCanExecuteChanged();
    }
}
