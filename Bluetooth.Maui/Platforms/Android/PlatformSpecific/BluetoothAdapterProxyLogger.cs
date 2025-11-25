using ScanMode = Android.Bluetooth.ScanMode;

namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Provides logging and property tracking for Android BluetoothAdapter operations.
/// Inherits from BaseBindableObject to support property change notifications and logging.
/// </summary>
public class BluetoothAdapterProxyLogger : BaseBindableObject
{
    /// <summary>
    /// Refreshes all BluetoothAdapter property values from the native adapter.
    /// Only updates values if the BluetoothAdapter has been initialized.
    /// </summary>
    public void RefreshValues()
    {
        if (!BluetoothAdapterProxy.IsInitialized)
        {
            ClearAllValues();
            return;
        }

        var adapter = BluetoothAdapterProxy.BluetoothAdapter;

        // Basic adapter properties
        BluetoothAdapterState = adapter.State;
        BluetoothAdapterIsEnabled = adapter.IsEnabled;
        BluetoothAdapterName = adapter.Name ?? string.Empty;
        BluetoothAdapterAddress = adapter.Address ?? string.Empty;

        // Scanning and discovery properties
        BluetoothAdapterScanMode = adapter.ScanMode;
        BluetoothAdapterIsDiscovering = adapter.IsDiscovering;

        // Advertisement support properties
        BluetoothAdapterIsMultipleAdvertisementSupported = adapter.IsMultipleAdvertisementSupported;
        BluetoothAdapterIsOffloadedFilteringSupported = adapter.IsOffloadedFilteringSupported;
        BluetoothAdapterIsOffloadedScanBatchingSupported = adapter.IsOffloadedScanBatchingSupported;

        // Android API 26+ properties (Bluetooth 5.0 features)
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            BluetoothAdapterIsLe2MPhySupported = adapter.IsLe2MPhySupported;
            BluetoothAdapterIsLeCodedPhySupported = adapter.IsLeCodedPhySupported;
            BluetoothAdapterIsLeExtendedAdvertisingSupported = adapter.IsLeExtendedAdvertisingSupported;
            BluetoothAdapterIsLePeriodicAdvertisingSupported = adapter.IsLePeriodicAdvertisingSupported;
        }

        // Android API 33+ properties
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            BluetoothAdapterMaxConnectedAudioDevices = adapter.MaxConnectedAudioDevices;
        }

        // Additional useful properties
        RefreshBondedDevicesCount();
    }

    /// <summary>
    /// Clears all property values to their default states.
    /// This is useful when the BluetoothAdapter is not available or not initialized.
    /// </summary>
    public void ClearAllValues()
    {
        // Basic adapter properties
        BluetoothAdapterState = State.Off;
        BluetoothAdapterIsEnabled = false;
        BluetoothAdapterName = string.Empty;
        BluetoothAdapterAddress = string.Empty;

        // Scanning and discovery properties
        BluetoothAdapterScanMode = ScanMode.None;
        BluetoothAdapterIsDiscovering = false;

        // Advertisement support properties
        BluetoothAdapterIsMultipleAdvertisementSupported = false;
        BluetoothAdapterIsOffloadedFilteringSupported = false;
        BluetoothAdapterIsOffloadedScanBatchingSupported = false;

        // Bluetooth 5.0 features
        BluetoothAdapterIsLe2MPhySupported = false;
        BluetoothAdapterIsLeCodedPhySupported = false;
        BluetoothAdapterIsLeExtendedAdvertisingSupported = false;
        BluetoothAdapterIsLePeriodicAdvertisingSupported = false;

        // Audio properties
        BluetoothAdapterMaxConnectedAudioDevices = 0;

        // Device management properties
        BluetoothAdapterBondedDevicesCount = 0;
    }

    /// <summary>
    /// Refreshes the count of bonded (paired) devices.
    /// </summary>
    private void RefreshBondedDevicesCount()
    {
        try
        {
            if (BluetoothAdapterProxy.IsInitialized && BluetoothAdapterProxy.BluetoothAdapter.IsEnabled)
            {
                var bondedDevices = BluetoothAdapterProxy.BluetoothAdapter.BondedDevices;
                BluetoothAdapterBondedDevicesCount = bondedDevices?.Count ?? 0;
            }
            else
            {
                BluetoothAdapterBondedDevicesCount = 0;
            }
        }
        catch (Java.Lang.SecurityException)
        {
            // If we can't access bonded devices due to permissions, set to 0
            BluetoothAdapterBondedDevicesCount = 0;
        }
        catch (InvalidOperationException)
        {
            // If adapter is in an invalid state, set to 0
            BluetoothAdapterBondedDevicesCount = 0;
        }
    }

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
    public ScanMode BluetoothAdapterScanMode
    {
        get => GetValue(ScanMode.None);
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
