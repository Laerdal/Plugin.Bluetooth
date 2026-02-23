namespace Bluetooth.Maui.Platforms.Win.NativeObjects;

/// <summary>
///     Wrapper class for Windows Bluetooth radio that provides singleton-like access pattern
///     and manages radio state, kind, and name properties.
/// </summary>
public partial class RadioWrapper : BaseBindableObject, IRadioWrapper, IDisposable
{
    private readonly ITicker _ticker;

    private Windows.Devices.Radios.Radio? _radio;

    private readonly IBluetoothAdapterWrapper _bluetoothAdapterWrapper;

    private IDisposable? _refreshSubscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RadioWrapper" /> class.
    /// </summary>
    /// <param name="bluetoothAdapterWrapper">The Bluetooth adapter wrapper to retrieve the radio from.</param>
    /// <param name="ticker">The ticker for scheduling periodic updates of radio properties.</param>
    /// <param name="logger">An optional logger for logging radio operations and errors.</param>
    public RadioWrapper(IBluetoothAdapterWrapper bluetoothAdapterWrapper, ITicker ticker, ILogger<IRadioWrapper>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        ArgumentNullException.ThrowIfNull(bluetoothAdapterWrapper);
        _ticker = ticker;
        _bluetoothAdapterWrapper = bluetoothAdapterWrapper;
    }

    #region Radio

    private TaskCompletionSource<Windows.Devices.Radios.Radio>? RadioInitializationTcs { get; set; }

    /// <inheritdoc />
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

    /// <summary>
    /// Waits asynchronously for the radio to reach a specific state.
    /// </summary>
    /// <param name="state">The target radio state to wait for.</param>
    /// <param name="timeout">The timeout for the wait operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that completes when the target state is reached.</returns>
    public ValueTask<RadioState> WaitForRadioStateAsync(RadioState state, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(RadioState), state, timeout, cancellationToken);
    }

    /// <summary>
    /// Waits asynchronously for the radio state to be known (not <see cref="RadioState.Unknown"/>).
    /// </summary>
    /// <param name="timeout">The timeout for the wait operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that completes when the state becomes known.</returns>
    public ValueTask<RadioState> WaitForRadioStateToBeKnownAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeDifferentThanValue(nameof(RadioState), RadioState.Unknown, timeout, cancellationToken);
    }

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
        if (_radio == null)
        {
            return;
        }

        RadioState = _radio.State;
        RadioKind = _radio.Kind;
        RadioName = _radio.Name;
    }

    /// <inheritdoc />
    public RadioState RadioState
    {
        get => GetValue(RadioState.Unknown);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public RadioKind RadioKind
    {
        get => GetValue(RadioKind.Other);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public string RadioName
    {
        get => GetValue(string.Empty);
        private set => SetValue(value);
    }

}
