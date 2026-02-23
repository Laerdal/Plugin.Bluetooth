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

    /// <inheritdoc />
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
        if (_bluetoothAdapter == null)
        {
            return;
        }

        BluetoothAdapterDeviceId = _bluetoothAdapter.DeviceId;
        BluetoothAdapterBluetoothAddress = _bluetoothAdapter.BluetoothAddress;
        BluetoothAdapterIsAdvertisementOffloadSupported = _bluetoothAdapter.IsAdvertisementOffloadSupported;
        BluetoothAdapterIsLowEnergySupported = _bluetoothAdapter.IsLowEnergySupported;
        BluetoothAdapterIsClassicSupported = _bluetoothAdapter.IsClassicSupported;
        BluetoothAdapterAreLowEnergySecureConnectionsSupported = _bluetoothAdapter.AreLowEnergySecureConnectionsSupported;
        BluetoothAdapterIsPeripheralRoleSupported = _bluetoothAdapter.IsPeripheralRoleSupported;
        BluetoothAdapterIsCentralRoleSupported = _bluetoothAdapter.IsCentralRoleSupported;
        BluetoothAdapterAreClassicSecureConnectionsSupported = _bluetoothAdapter.AreClassicSecureConnectionsSupported;
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
        {
            BluetoothAdapterMaxAdvertisementDataLength = _bluetoothAdapter.MaxAdvertisementDataLength;
            BluetoothAdapterIsExtendedAdvertisingSupported = _bluetoothAdapter.IsExtendedAdvertisingSupported;
        }
    }

    #region BluetoothAdapter

    /// <inheritdoc />
    public string BluetoothAdapterDeviceId
    {
        get => GetValue(string.Empty);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public ulong BluetoothAdapterBluetoothAddress
    {
        get => GetValue(0ul);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public bool BluetoothAdapterIsAdvertisementOffloadSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public bool BluetoothAdapterIsLowEnergySupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public bool BluetoothAdapterIsClassicSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public bool BluetoothAdapterAreClassicSecureConnectionsSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public bool BluetoothAdapterAreLowEnergySecureConnectionsSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public bool BluetoothAdapterIsPeripheralRoleSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public bool BluetoothAdapterIsCentralRoleSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public uint BluetoothAdapterMaxAdvertisementDataLength
    {
        get => GetValue(0u);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public bool BluetoothAdapterIsExtendedAdvertisingSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    #endregion

}
