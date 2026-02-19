namespace Bluetooth.Maui.Platforms.Droid.NativeObjects;

/// <summary>
/// Wrapper class for Android's BluetoothAdapter to provide a consistent interface for Bluetooth operations in the application.
/// This class retrieves the BluetoothAdapter instance from the BluetoothManager wrapper and ensures it is available when accessed.
/// </summary>
public partial class BluetoothAdapterWrapper : BaseBindableObject, IBluetoothAdapterWrapper, IDisposable
{
    private Android.Bluetooth.BluetoothAdapter? _bluetoothAdapter;

    private readonly ITicker _ticker;

    private readonly IBluetoothManagerWrapper _bluetoothManagerWrapper;

    private IDisposable? _refreshSubscription;

    /// <inheritdoc />
    public Android.Bluetooth.BluetoothAdapter BluetoothAdapter
    {
        get
        {
            if (_bluetoothAdapter == null)
            {
                _bluetoothAdapter = _bluetoothManagerWrapper.BluetoothManager.Adapter;
                if (_bluetoothAdapter == null)
                {
                    throw new InvalidOperationException("BluetoothAdapter is null - ensure Bluetooth is available on this device");
                }
                _refreshSubscription = _ticker?.Register("bluetooth_adapter_wrapper_refresh", TimeSpan.FromSeconds(1), RefreshAdapterProperties, true);
            }
            return _bluetoothAdapter;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothAdapterWrapper"/> class with the specified BluetoothManager wrapper.
    /// </summary>
    /// <param name="bluetoothManagerWrapper">The BluetoothManager wrapper to use for accessing the BluetoothAdapter.</param>
    /// <param name="ticker">The ticker for scheduling periodic updates of adapter properties.</param>
    /// <param name="logger">An optional logger for logging adapter operations and errors.</param>
    public BluetoothAdapterWrapper(IBluetoothManagerWrapper bluetoothManagerWrapper, ITicker ticker, ILogger<IBluetoothAdapterWrapper>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(bluetoothManagerWrapper);
        ArgumentNullException.ThrowIfNull(ticker);
        
        _bluetoothManagerWrapper = bluetoothManagerWrapper;
        _ticker = ticker;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the BluetoothAdapter and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshSubscription?.Dispose();
            _bluetoothAdapter?.Dispose();
        }
    }

    /// <summary>
    /// Attempts to enable the Bluetooth adapter.
    /// </summary>
    /// <returns>True if the adapter is enabled; otherwise, false.</returns>
    public bool TryEnableAdapter()
    {
        try
        {
            if (!BluetoothAdapter.IsEnabled && !OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                BluetoothAdapter.Enable();
            }
            return BluetoothAdapter.IsEnabled;
        }
        catch (Java.Lang.SecurityException ex)
        {
            // Handle permission-related issues
            System.Diagnostics.Debug.WriteLine($"Security exception when enabling BluetoothAdapter: {ex.Message}");
            return false;
        }
        catch (InvalidOperationException ex)
        {
            // Handle adapter-related issues
            System.Diagnostics.Debug.WriteLine($"Invalid operation when enabling BluetoothAdapter: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Attempts to disable the Bluetooth adapter.
    /// </summary>
    /// <returns>True if the adapter is disabled; otherwise, false.</returns>
    public bool TryDisableAdapter()
    {
        try
        {
            if (BluetoothAdapter.IsEnabled && !OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                BluetoothAdapter.Disable();
            }
            return !BluetoothAdapter.IsEnabled;
        }
        catch (Java.Lang.SecurityException ex)
        {
            // Handle permission-related issues
            System.Diagnostics.Debug.WriteLine($"Security exception when disabling BluetoothAdapter: {ex.Message}");
            return false;
        }
        catch (InvalidOperationException ex)
        {
            // Handle adapter-related issues
            System.Diagnostics.Debug.WriteLine($"Invalid operation when disabling BluetoothAdapter: {ex.Message}");
            return false;
        }
    }

    #region Basic Adapter Properties

    /// <summary>
    /// Refreshes the properties of the Bluetooth adapter by querying the native Android BluetoothAdapter instance.
    /// This method is called periodically to ensure that the properties reflect the current state of the adapter.
    /// </summary>
    private void RefreshAdapterProperties()
    {
        // Basic adapter properties
        BluetoothAdapterState = BluetoothAdapter.State;
        BluetoothAdapterIsEnabled = BluetoothAdapter.IsEnabled;
        BluetoothAdapterName = BluetoothAdapter.Name ?? string.Empty;
        BluetoothAdapterAddress = BluetoothAdapter.Address ?? string.Empty;

        // Scanning and discovery properties
        BluetoothAdapterScanMode = BluetoothAdapter.ScanMode;
        BluetoothAdapterIsDiscovering = BluetoothAdapter.IsDiscovering;

        // Refresh bonded devices count
        BluetoothAdapterBondedDevicesCount = BluetoothAdapter.BondedDevices?.Count ?? 0;

        // Advertisement support properties
        BluetoothAdapterIsMultipleAdvertisementSupported = BluetoothAdapter.IsMultipleAdvertisementSupported;
        BluetoothAdapterIsOffloadedFilteringSupported = BluetoothAdapter.IsOffloadedFilteringSupported;
        BluetoothAdapterIsOffloadedScanBatchingSupported = BluetoothAdapter.IsOffloadedScanBatchingSupported;

        // Android API 26+ properties (Bluetooth 5.0 features)
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            BluetoothAdapterIsLe2MPhySupported = BluetoothAdapter.IsLe2MPhySupported;
            BluetoothAdapterIsLeCodedPhySupported = BluetoothAdapter.IsLeCodedPhySupported;
            BluetoothAdapterIsLeExtendedAdvertisingSupported = BluetoothAdapter.IsLeExtendedAdvertisingSupported;
            BluetoothAdapterIsLePeriodicAdvertisingSupported = BluetoothAdapter.IsLePeriodicAdvertisingSupported;
        }

        // Android API 33+ properties
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            BluetoothAdapterMaxConnectedAudioDevices = BluetoothAdapter.MaxConnectedAudioDevices;
        }
    }

    /// <summary>
    /// Gets the current state of the Bluetooth adapter.
    /// </summary>
    public State BluetoothAdapterState
    {
        get => GetValue(State.Off);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter is enabled.
    /// </summary>
    public bool BluetoothAdapterIsEnabled
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the friendly name of the Bluetooth adapter.
    /// </summary>
    public string BluetoothAdapterName
    {
        get => GetValue(string.Empty);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the hardware address (MAC address) of the Bluetooth adapter.
    /// </summary>
    public string BluetoothAdapterAddress
    {
        get => GetValue(string.Empty);
        private set => SetValue(value);
    }

    #endregion

    #region Scanning and Discovery Properties

    /// <summary>
    /// Gets the current scan mode of the Bluetooth adapter.
    /// </summary>
    public Android.Bluetooth.ScanMode BluetoothAdapterScanMode
    {
        get => GetValue(Android.Bluetooth.ScanMode.None);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter is currently discovering devices.
    /// </summary>
    public bool BluetoothAdapterIsDiscovering
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    #endregion

    #region Advertisement Support Properties

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter supports multiple simultaneous advertisements.
    /// </summary>
    public bool BluetoothAdapterIsMultipleAdvertisementSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter supports offloaded filtering of scan results.
    /// </summary>
    public bool BluetoothAdapterIsOffloadedFilteringSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter supports offloaded batching of scan results.
    /// </summary>
    public bool BluetoothAdapterIsOffloadedScanBatchingSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    #endregion

    #region Bluetooth 5.0 Features (Android API 26+)

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter supports Bluetooth Low Energy 2M PHY.
    /// Available on Android API 26 and later.
    /// </summary>
    public bool BluetoothAdapterIsLe2MPhySupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter supports Bluetooth Low Energy Coded PHY.
    /// Available on Android API 26 and later.
    /// </summary>
    public bool BluetoothAdapterIsLeCodedPhySupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter supports Bluetooth Low Energy extended advertising.
    /// Available on Android API 26 and later.
    /// </summary>
    public bool BluetoothAdapterIsLeExtendedAdvertisingSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth adapter supports Bluetooth Low Energy periodic advertising.
    /// Available on Android API 26 and later.
    /// </summary>
    public bool BluetoothAdapterIsLePeriodicAdvertisingSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    #endregion

    #region Audio Properties (Android API 33+)

    /// <summary>
    /// Gets the maximum number of connected audio devices supported by the Bluetooth adapter.
    /// Available on Android API 33 and later.
    /// </summary>
    public int BluetoothAdapterMaxConnectedAudioDevices
    {
        get => GetValue(0);
        private set => SetValue(value);
    }

    #endregion

    #region Device Management Properties

    /// <summary>
    /// Gets the number of bonded (paired) devices associated with this Bluetooth adapter.
    /// </summary>
    public int BluetoothAdapterBondedDevicesCount
    {
        get => GetValue(0);
        private set => SetValue(value);
    }

    #endregion

}