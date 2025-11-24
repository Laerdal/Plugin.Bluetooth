namespace Plugin.Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Proxy class for Windows Radio that provides event handling and singleton-like access patterns
/// for Bluetooth radio state management.
/// </summary>
public partial class RadioProxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RadioProxy"/> class.
    /// </summary>
    /// <param name="radio">The native Windows radio instance.</param>
    /// <param name="radioProxyDelegate">The delegate for handling radio events.</param>
    private RadioProxy(Radio radio, IRadioProxyDelegate radioProxyDelegate)
    {
        RadioProxyDelegate = radioProxyDelegate;
        Radio = radio;
        Radio.StateChanged += Radio_StateChanged;
    }

    /// <summary>
    /// Gets the delegate responsible for handling radio events.
    /// </summary>
    private IRadioProxyDelegate RadioProxyDelegate { get; }

    /// <summary>
    /// Gets the native Windows radio instance.
    /// </summary>
    public Radio Radio { get; }

    /// <summary>
    /// Handles radio state change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The radio that changed state.</param>
    /// <param name="args">The event arguments (not used).</param>
    private void Radio_StateChanged(Radio sender, object args)
    {
        RadioProxyDelegate.OnRadioStateChanged(sender.State);
    }

    #region Static instance

    /// <summary>
    /// Gets or sets the cached radio instance.
    /// </summary>
    private static Radio? RadioInstance { get; set; }

    /// <summary>
    /// Gets a singleton instance of the <see cref="RadioProxy"/> associated with the specified Bluetooth adapter.
    /// </summary>
    /// <param name="bluetoothAdapterProxy">The Bluetooth adapter proxy to get the radio from.</param>
    /// <param name="radioProxyDelegate">The delegate for handling radio events.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the radio proxy instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="bluetoothAdapterProxy"/> is null.</exception>
    /// <exception cref="PermissionException">Thrown when the radio cannot be retrieved, indicating missing Bluetooth capability.</exception>
    public async static Task<RadioProxy> GetInstanceAsync(BluetoothAdapterProxy bluetoothAdapterProxy, IRadioProxyDelegate radioProxyDelegate)
    {
        ArgumentNullException.ThrowIfNull(bluetoothAdapterProxy);
        RadioInstance ??= await bluetoothAdapterProxy.BluetoothAdapter.GetRadioAsync();
        if (RadioInstance == null)
        {
            throw new PermissionException("BluetoothAdapter.GetRadioAsync = null, Did you forget to add '<DeviceCapability Name=\"bluetooth\" />' in your Manifest's Capabilities ?");
        }

        return new RadioProxy(RadioInstance, radioProxyDelegate);
    }

    #endregion
}
