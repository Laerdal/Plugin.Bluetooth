namespace Bluetooth.Maui.Platforms.Win.NativeObjects;

/// <summary>
///     Proxy class for Windows Bluetooth adapter that provides a singleton-like access pattern
///     and delegate-based communication for Bluetooth operations.
/// </summary>
public partial class BluetoothAdapterWrapper : BaseBindableObject, IBluetoothAdapterWrapper, IDisposable
{

    private readonly ITicker _ticker;

    private Windows.Devices.Bluetooth.BluetoothAdapter? _bluetoothAdapter;

    private IDisposable? _refreshSubscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothAdapterWrapper" /> class with the specified BluetoothManager wrapper.
    /// </summary>
    /// <param name="ticker">The ticker for scheduling periodic updates of adapter properties.</param>
    /// <param name="logger">An optional logger for logging adapter operations and errors.</param>
    public BluetoothAdapterWrapper(ITicker ticker, ILogger<IBluetoothAdapterWrapper>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        _ticker = ticker;
    }

    #region BluetoothAdapter

    private TaskCompletionSource<Windows.Devices.Bluetooth.BluetoothAdapter>? BluetoothAdapterInitializationTcs { get; set; }

    /// <summary>
    ///    Gets the Bluetooth adapter proxy, ensuring that only one instance is created and shared across the application.
    /// </summary>
    public async ValueTask<Windows.Devices.Bluetooth.BluetoothAdapter> GetBluetoothAdapterAsync(CancellationToken cancellationToken = default)
    {
        if (_bluetoothAdapter != null)
        {
            return _bluetoothAdapter;
        }
        if (BluetoothAdapterInitializationTcs != null)
        {
            return await BluetoothAdapterInitializationTcs.Task.ConfigureAwait(false);
        }
        BluetoothAdapterInitializationTcs = new TaskCompletionSource<Windows.Devices.Bluetooth.BluetoothAdapter>();
        try
        {
            _bluetoothAdapter = await Windows.Devices.Bluetooth.BluetoothAdapter.GetDefaultAsync().AsTask(cancellationToken).ConfigureAwait(false);
            if (_bluetoothAdapter == null)
            {
                throw new PermissionException("BluetoothAdapter.GetDefaultAsync = null, Did you forget to add '<DeviceCapability Name=\"bluetooth\" />' in your Manifest's Capabilities?");
            }

            _refreshSubscription = _ticker?.Register("bluetooth_adapter_wrapper_refresh", TimeSpan.FromSeconds(1), RefreshAdapterProperties, true);

            BluetoothAdapterInitializationTcs.TrySetResult(_bluetoothAdapter);
            return _bluetoothAdapter;
        }
        catch (Exception e)
        {
            BluetoothAdapterInitializationTcs.TrySetException(e);
            throw;
        }
        finally
        {
            BluetoothAdapterInitializationTcs = null;
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

    private void RefreshAdapterProperties()
    {
        // Adapter properties
        BluetoothAdapterIsAdvertisementOffloadSupported = _bluetoothAdapter?.IsAdvertisementOffloadSupported ?? false;
        BluetoothAdapterIsLowEnergySupported = _bluetoothAdapter?.IsLowEnergySupported ?? false;
        BluetoothAdapterIsClassicSupported = _bluetoothAdapter?.IsClassicSupported ?? false;
        BluetoothAdapterAreLowEnergySecureConnectionsSupported = _bluetoothAdapter?.AreLowEnergySecureConnectionsSupported ?? false;
        BluetoothAdapterIsPeripheralRoleSupported = _bluetoothAdapter?.IsPeripheralRoleSupported ?? false;
        BluetoothAdapterAreClassicSecureConnectionsSupported = _bluetoothAdapter?.AreClassicSecureConnectionsSupported ?? false;
    }

    #region BluetoothAdapter

    /// <summary>
    /// Gets a value indicating whether Bluetooth advertisement offload is supported.
    /// </summary>
    public bool BluetoothAdapterIsAdvertisementOffloadSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether Bluetooth Low Energy is supported.
    /// </summary>
    public bool BluetoothAdapterIsLowEnergySupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether classic Bluetooth is supported.
    /// </summary>
    public bool BluetoothAdapterIsClassicSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether classic Bluetooth secure connections are supported.
    /// </summary>
    public bool BluetoothAdapterAreClassicSecureConnectionsSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether Bluetooth Low Energy secure connections are supported.
    /// </summary>
    public bool BluetoothAdapterAreLowEnergySecureConnectionsSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the peripheral role is supported.
    /// </summary>
    public bool BluetoothAdapterIsPeripheralRoleSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    #endregion

}