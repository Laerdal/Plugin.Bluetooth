using Bluetooth.Core.Infrastructure.Scheduling;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid;

/// <summary>
/// Android implementation of the Bluetooth adapter.
/// </summary>
public abstract class BluetoothAdapter : BaseBluetoothAdapter, IDisposable
{
    /// <inheritdoc/>
    protected BluetoothAdapter(ITicker ticker, ILogger? logger = null) : base(logger)
    {
        _lazyBluetoothManager = new Lazy<Android.Bluetooth.BluetoothManager>(GetBluetoothManager);
        _lazyBluetoothAdapter = new Lazy<Android.Bluetooth.BluetoothAdapter>(GetBluetoothAdapter);

        ArgumentNullException.ThrowIfNull(ticker);
        _refreshAdapterPropertiesSubscription = ticker.Register("refresh_adapter_properties", TimeSpan.FromSeconds(1), RefreshAdapterProperties, true);
    }

    private readonly IDisposable _refreshAdapterPropertiesSubscription;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
            }

    /// <summary>
    /// Releases the unmanaged resources used by the AndroidBluetoothAdapter and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshAdapterPropertiesSubscription.Dispose();
        }
    }


    #region BluetoothManager

    private Android.Bluetooth.BluetoothManager GetBluetoothManager()
    {
        var manager = Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService) as Android.Bluetooth.BluetoothManager;
        return manager ?? throw new InvalidOperationException("BluetoothManager is null - ensure Bluetooth is available on this device");
    }

    private readonly Lazy<Android.Bluetooth.BluetoothManager> _lazyBluetoothManager;

    /// <summary>
    /// Gets the BluetoothManager instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when BluetoothManager is not available on the device.</exception>
    public Android.Bluetooth.BluetoothManager BluetoothManager => _lazyBluetoothManager.Value;

    #endregion

    #region BluetoothAdapter

    private Android.Bluetooth.BluetoothAdapter GetBluetoothAdapter()
    {
        return BluetoothManager.Adapter ?? throw new InvalidOperationException("BluetoothAdapter is null - ensure Bluetooth is available on this device");
    }

    private readonly Lazy<Android.Bluetooth.BluetoothAdapter> _lazyBluetoothAdapter;

    /// <summary>
    /// Gets the BluetoothAdapter instance.
    /// </summary>
    public Android.Bluetooth.BluetoothAdapter AndroidBluetoothAdapter => _lazyBluetoothAdapter.Value;

    /// <summary>
    /// Attempts to enable the Bluetooth adapter.
    /// </summary>
    /// <returns>True if the adapter is enabled; otherwise, false.</returns>
    public bool TryEnableAdapter()
    {
        try
        {
            if (!AndroidBluetoothAdapter.IsEnabled && !OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                AndroidBluetoothAdapter.Enable();
            }
            return AndroidBluetoothAdapter.IsEnabled;
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
            if (AndroidBluetoothAdapter.IsEnabled && !OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                AndroidBluetoothAdapter.Disable();
            }
            return !AndroidBluetoothAdapter.IsEnabled;
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

    protected void RefreshAdapterProperties()
    {
        if (_lazyBluetoothAdapter.IsValueCreated == false)
        {
            return;
        }

        // Basic adapter properties
        BluetoothAdapterState = AndroidBluetoothAdapter.State;
        BluetoothAdapterIsEnabled = AndroidBluetoothAdapter.IsEnabled;
        BluetoothAdapterName = AndroidBluetoothAdapter.Name ?? string.Empty;
        BluetoothAdapterAddress = AndroidBluetoothAdapter.Address ?? string.Empty;

        // Scanning and discovery properties
        BluetoothAdapterScanMode = AndroidBluetoothAdapter.ScanMode;
        BluetoothAdapterIsDiscovering = AndroidBluetoothAdapter.IsDiscovering;

        // Refresh bonded devices count
        BluetoothAdapterBondedDevicesCount = AndroidBluetoothAdapter.BondedDevices?.Count ?? 0;

        // Advertisement support properties
        BluetoothAdapterIsMultipleAdvertisementSupported = AndroidBluetoothAdapter.IsMultipleAdvertisementSupported;
        BluetoothAdapterIsOffloadedFilteringSupported = AndroidBluetoothAdapter.IsOffloadedFilteringSupported;
        BluetoothAdapterIsOffloadedScanBatchingSupported = AndroidBluetoothAdapter.IsOffloadedScanBatchingSupported;

        // Android API 26+ properties (Bluetooth 5.0 features)
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            BluetoothAdapterIsLe2MPhySupported = AndroidBluetoothAdapter.IsLe2MPhySupported;
            BluetoothAdapterIsLeCodedPhySupported = AndroidBluetoothAdapter.IsLeCodedPhySupported;
            BluetoothAdapterIsLeExtendedAdvertisingSupported = AndroidBluetoothAdapter.IsLeExtendedAdvertisingSupported;
            BluetoothAdapterIsLePeriodicAdvertisingSupported = AndroidBluetoothAdapter.IsLePeriodicAdvertisingSupported;
        }

        // Android API 33+ properties
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            BluetoothAdapterMaxConnectedAudioDevices = AndroidBluetoothAdapter.MaxConnectedAudioDevices;
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

    #endregion
}
