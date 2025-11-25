using Java.Lang;

namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Provides a proxy for Android BluetoothAdapter operations with error handling and logging.
/// </summary>
public static class BluetoothAdapterProxy
{
    private readonly static Lazy<Android.Bluetooth.BluetoothAdapter> _lazyBluetoothAdapter = new Lazy<Android.Bluetooth.BluetoothAdapter>(() => BluetoothManagerProxy.BluetoothManager.Adapter ?? throw new InvalidOperationException("BluetoothManager.Adapter is null"));

    /// <summary>
    /// Gets the BluetoothAdapter instance.
    /// </summary>
    public static Android.Bluetooth.BluetoothAdapter BluetoothAdapter => _lazyBluetoothAdapter.Value;

    private readonly static Lazy<BluetoothAdapterProxyLogger> _lazyBluetoothAdapterLogger = new Lazy<BluetoothAdapterProxyLogger>(() => new BluetoothAdapterProxyLogger());

    /// <summary>
    /// Gets the logger instance for BluetoothAdapter operations.
    /// </summary>
    public static BluetoothAdapterProxyLogger Logger => _lazyBluetoothAdapterLogger.Value;

    /// <summary>
    /// Gets a value indicating whether the BluetoothAdapter has been initialized.
    /// </summary>
    public static bool IsInitialized => _lazyBluetoothAdapter.IsValueCreated;

    /// <summary>
    /// Attempts to enable the Bluetooth adapter.
    /// </summary>
    /// <returns>True if the adapter is enabled; otherwise, false.</returns>
    public static bool TryEnableAdapter()
    {
        try
        {
            if (!BluetoothAdapter.IsEnabled && !OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                BluetoothAdapter.Enable();
            }
            return BluetoothAdapter.IsEnabled;
        }
        catch (SecurityException ex)
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
    public static bool TryDisableAdapter()
    {
        try
        {
            if (BluetoothAdapter.IsEnabled && !OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                BluetoothAdapter.Disable();
            }
            return !BluetoothAdapter.IsEnabled;
        }
        catch (SecurityException ex)
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
}
