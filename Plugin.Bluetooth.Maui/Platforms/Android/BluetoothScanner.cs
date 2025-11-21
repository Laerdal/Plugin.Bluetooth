using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothScanner : BaseBluetoothScanner, ScanCallbackProxy.IScanner, BluetoothAdapterEventProxy.IAdapter, BluetoothDeviceEventProxy.IScanner
{
    public ScanCallbackProxy ScanCallbackProxy { get; }

    public BluetoothScanner()
    {
        // Callbacks
        ScanCallbackProxy = new ScanCallbackProxy(this);
    }

    #region Advertisement Filtering

    public Func<ScanCallbackType, ScanResult, bool> NativeAdvertisementFilter { get; set; } = DefaultNativeAdvertisementFilter;

    private static bool DefaultNativeAdvertisementFilter(ScanCallbackType callbackType, ScanResult result)
    {
        return true;
    }

    #endregion


    #region BluetoothAdapterEventProxy.IAdapter Implementation

    /// <inheritdoc/>
    public virtual void OnConnectionStateChanged(ProfileState oldState, ProfileState newState, Android.Bluetooth.BluetoothDevice? eDevice)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public virtual void OnDiscoverableRequested(int duration)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public virtual void OnDiscoveryFinished()
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public virtual void OnDiscoveryStarted()
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public virtual void OnEnableRequested()
    {
        // Placeholder for future implementation if needed
    }

    public virtual void OnLocalNameChanged(string? newName)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public virtual void OnScanModeChanged(Android.Bluetooth.ScanMode previousScanMode, Android.Bluetooth.ScanMode newScanMode)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public virtual void OnStateChanged(State previousState, State newState)
    {
        // Placeholder for future implementation if needed
    }

    #endregion

    #region BluetoothDeviceEventProxy.IScanner Implementation

    public BluetoothDeviceEventProxy.IDevice GetDevice(Android.Bluetooth.BluetoothDevice? nativeDevice)
    {
        ArgumentNullException.ThrowIfNull(nativeDevice);
        ArgumentNullException.ThrowIfNull(nativeDevice.Address);

        try
        {
            var match = Devices.Cast<BluetoothDevice>().SingleOrDefault(device => AreRepresentingTheSameObject(nativeDevice, device));

            return match ?? throw new DeviceNotFoundException(this, nativeDevice.Address);
        }
        catch (InvalidOperationException e)
        {
            var matches = Devices.OfType<BluetoothDevice>().Where(device => AreRepresentingTheSameObject(nativeDevice, device)).ToArray();
            throw new MultipleDevicesFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(Android.Bluetooth.BluetoothDevice nativeDevice, BluetoothDevice device)
    {
        return device.Id == nativeDevice.Address;
    }

    /// <inheritdoc/>
    public void OnDeviceFound(Android.Bluetooth.BluetoothDevice device)
    {
        // Placeholder for future implementation if needed
    }

    #endregion
}
