namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Proxy class for Windows Bluetooth adapter that provides a singleton-like access pattern
/// and delegate-based communication for Bluetooth operations.
/// </summary>
public partial class BluetoothAdapterProxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothAdapterProxy"/> class.
    /// </summary>
    /// <param name="bluetoothAdapter">The native Windows Bluetooth adapter instance.</param>
    /// <param name="bluetoothAdapterProxyDelegate">The delegate for handling Bluetooth adapter events.</param>
    private BluetoothAdapterProxy(Windows.Devices.Bluetooth.BluetoothAdapter bluetoothAdapter, IBluetoothAdapterProxyDelegate bluetoothAdapterProxyDelegate)
    {
        BluetoothAdapterProxyDelegate = bluetoothAdapterProxyDelegate;
        BluetoothAdapter = bluetoothAdapter;
    }

    /// <summary>
    /// Gets the delegate responsible for handling Bluetooth adapter events and operations.
    /// </summary>
    protected IBluetoothAdapterProxyDelegate BluetoothAdapterProxyDelegate { get; }

    /// <summary>
    /// Gets the native Windows Bluetooth adapter instance.
    /// </summary>
    public Windows.Devices.Bluetooth.BluetoothAdapter BluetoothAdapter { get; }

    #region Static instance

    /// <summary>
    /// Gets or sets the cached Bluetooth adapter instance.
    /// </summary>
    private static Windows.Devices.Bluetooth.BluetoothAdapter? BluetoothAdapterInstance { get; set; }

    /// <summary>
    /// Gets a singleton instance of the <see cref="BluetoothAdapterProxy"/> with the specified delegate.
    /// </summary>
    /// <param name="bluetoothAdapterProxyDelegate">The delegate for handling Bluetooth adapter events.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the proxy instance.</returns>
    /// <exception cref="PermissionException">
    /// Thrown when the default Bluetooth adapter is null, indicating missing Bluetooth capability in the app manifest.
    /// </exception>
    public async static Task<BluetoothAdapterProxy> GetInstanceAsync(IBluetoothAdapterProxyDelegate bluetoothAdapterProxyDelegate)
    {
        BluetoothAdapterInstance ??= await Windows.Devices.Bluetooth.BluetoothAdapter.GetDefaultAsync();
        if (BluetoothAdapterInstance == null)
        {
            throw new PermissionException("BluetoothAdapter.GetDefaultAsync = null, Did you forget to add '<DeviceCapability Name=\"bluetooth\" />' in your Manifest's Capabilities?");
        }

        return new BluetoothAdapterProxy(BluetoothAdapterInstance, bluetoothAdapterProxyDelegate);
    }

    #endregion
}
