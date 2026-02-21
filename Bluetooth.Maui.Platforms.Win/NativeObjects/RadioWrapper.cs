namespace Bluetooth.Maui.Platforms.Win.NativeObjects;

public partial class RadioWrapper : BaseBindableObject, IRadioWrapper, IDisposable
{
    private readonly ITicker _ticker;

    private Windows.Devices.Radios.Radio? _radio;

    private readonly IBluetoothAdapterWrapper _bluetoothAdapterWrapper;
    
    private IDisposable? _refreshSubscription;
    
    private RadioWrapper(IBluetoothAdapterWrapper bluetoothAdapterWrapper, ITicker ticker, ILogger<IBluetoothAdapterWrapper>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        ArgumentNullException.ThrowIfNull(bluetoothAdapterWrapper);
        _ticker = ticker;
        _bluetoothAdapterWrapper = bluetoothAdapterWrapper;
    }

    #region Radio

    private TaskCompletionSource<Windows.Devices.Radios.Radio>? RadioInitializationTcs { get; set; }

    /// <summary>
    ///    Gets the Bluetooth adapter proxy, ensuring that only one instance is created and shared across the application.
    /// </summary>
    public async ValueTask<Windows.Devices.Radios.Radio> GetRadioAsync(CancellationToken cancellationToken = default)
    {
        if (_radio != null)
        {
            return _radio;
        }
        if (RadioInitializationTcs != null)
        {
            return await RadioInitializationTcs.Task.ConfigureAwait(false);
        }
        RadioInitializationTcs = new TaskCompletionSource<Windows.Devices.Radios.Radio>();
        try
        {
            var bluetoothAdapter = await _bluetoothAdapterWrapper.GetBluetoothAdapterAsync(cancellationToken).ConfigureAwait(false);
            _radio = await bluetoothAdapter.GetRadioAsync().AsTask(cancellationToken).ConfigureAwait(false);
            if (_radio == null)
            {
                throw new PermissionException("BluetoothAdapter.GetRadioAsync = null, Did you forget to add '<DeviceCapability Name=\"bluetooth\" />' in your Manifest's Capabilities?");
            }

            _refreshSubscription = _ticker?.Register("radio_wrapper_refresh", TimeSpan.FromSeconds(1), RefreshRadioProperties, true);

            RadioInitializationTcs.TrySetResult(_radio);
            return _radio;
        }
        catch (Exception e)
        {
            RadioInitializationTcs.TrySetException(e);
            throw;
        }
        finally
        {
            RadioInitializationTcs = null;
        }
    }

    #endregion
    
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    ///     Releases the unmanaged resources used by the BluetoothAdapter and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshSubscription?.Dispose();
        }
    }
    
    private void RefreshRadioProperties()
    {
        // Adapter properties
        RadioState = _radio?.State ?? RadioState.Unknown;
        RadioKind = _radio?.Kind ?? RadioKind.Other;
        RadioName = _radio?.Name ?? string.Empty;
    }
    
    /// <summary>
    /// Gets the current state of the Bluetooth radio.
    /// </summary>
    public RadioState RadioState
    {
        get => GetValue(RadioState.Unknown);
        private set => SetValue(value);
    }
    /// <summary>
    /// Gets the kind of the Bluetooth radio.
    /// </summary>
    public RadioKind RadioKind
    {
        get => GetValue(RadioKind.Other);
        private set => SetValue(value);
    }
    
    /// <summary>
    /// Gets the name of the Bluetooth radio.
    /// </summary>
    public string RadioName
    {
        get => GetValue(string.Empty);
        private set => SetValue(value);
    }

}