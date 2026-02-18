using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <inheritdoc/>
public class BluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc/>
    public BluetoothAdapter(ILogger? logger = null)
        : base(logger)
    {
        NativeBluetoothAdapter = BluetoothManagerProxy.BluetoothManager.Adapter ?? throw new InvalidOperationException("BluetoothManager.Adapter is null");
    }

    /// <inheritdoc/>
    protected override void NativeRefreshValues()
    {
        ArgumentNullException.ThrowIfNull(NativeBluetoothAdapter);

        // Basic adapter properties
        BluetoothAdapterState = NativeBluetoothAdapter.State;
        BluetoothAdapterIsEnabled = NativeBluetoothAdapter.IsEnabled;
        BluetoothAdapterName = NativeBluetoothAdapter.Name ?? string.Empty;
        BluetoothAdapterAddress = NativeBluetoothAdapter.Address ?? string.Empty;

        // Scanning and discovery properties
        BluetoothAdapterScanMode = NativeBluetoothAdapter.ScanMode;
        BluetoothAdapterIsDiscovering = NativeBluetoothAdapter.IsDiscovering;

        // Refresh bonded devices count
        BluetoothAdapterBondedDevicesCount = NativeBluetoothAdapter.BondedDevices?.Count ?? 0;

        // Advertisement support properties
        BluetoothAdapterIsMultipleAdvertisementSupported = NativeBluetoothAdapter.IsMultipleAdvertisementSupported;
        BluetoothAdapterIsOffloadedFilteringSupported = NativeBluetoothAdapter.IsOffloadedFilteringSupported;
        BluetoothAdapterIsOffloadedScanBatchingSupported = NativeBluetoothAdapter.IsOffloadedScanBatchingSupported;

        // Android API 26+ properties (Bluetooth 5.0 features)
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            BluetoothAdapterIsLe2MPhySupported = NativeBluetoothAdapter.IsLe2MPhySupported;
            BluetoothAdapterIsLeCodedPhySupported = NativeBluetoothAdapter.IsLeCodedPhySupported;
            BluetoothAdapterIsLeExtendedAdvertisingSupported = NativeBluetoothAdapter.IsLeExtendedAdvertisingSupported;
            BluetoothAdapterIsLePeriodicAdvertisingSupported = NativeBluetoothAdapter.IsLePeriodicAdvertisingSupported;
        }

        // Android API 33+ properties
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            BluetoothAdapterMaxConnectedAudioDevices = NativeBluetoothAdapter.MaxConnectedAudioDevices;
        }
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsBluetoothOn()
    {
        IsBluetoothOn = NativeBluetoothAdapter.IsEnabled;
    }

    #region Native Adapter Access

    /// <summary>
    /// Gets the BluetoothAdapter instance.
    /// </summary>
    public Android.Bluetooth.BluetoothAdapter NativeBluetoothAdapter { get; }

    /// <summary>
    /// Attempts to enable the Bluetooth adapter.
    /// </summary>
    /// <returns>True if the adapter is enabled; otherwise, false.</returns>
    public bool TryEnableAdapter()
    {
        try
        {
            if (!NativeBluetoothAdapter.IsEnabled && !OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                NativeBluetoothAdapter.Enable();
            }
            return NativeBluetoothAdapter.IsEnabled;
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
            if (NativeBluetoothAdapter.IsEnabled && !OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                NativeBluetoothAdapter.Disable();
            }
            return !NativeBluetoothAdapter.IsEnabled;
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

    #endregion

    #region Basic Adapter Properties

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
