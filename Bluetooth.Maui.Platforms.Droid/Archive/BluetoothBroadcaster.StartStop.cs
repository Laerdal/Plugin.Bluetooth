using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    /// <remarks>
    /// On Android, checks if the Bluetooth adapter scan mode is <see cref="Android.Bluetooth.ScanMode.ConnectableDiscoverable"/>.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = ((BluetoothAdapter)Adapter).NativeBluetoothAdapter.ScanMode == Android.Bluetooth.ScanMode.ConnectableDiscoverable;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Starts BLE advertising using the Android <see cref="BluetoothLeAdvertiser"/> with the configured settings and data.
    /// Also initializes the GATT server callback proxy.
    /// </remarks>
    protected override ValueTask NativeStartAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        BluetoothLeAdvertiser.StartAdvertising(AdvertiseSettings, AdvertiseData, ScanResponseData, AdvertiseCallbackProxy);
        BluetoothGattServerCallbackProxy = new BluetoothGattServerCallbackProxy(this);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Gets or sets the advertise settings that control advertising mode, power level, and other parameters.
    /// </summary>
    public AdvertiseSettings? AdvertiseSettings
    {
        get => GetValue<AdvertiseSettings?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the advertise data to be broadcast in the advertisement packet.
    /// </summary>
    public AdvertiseData? AdvertiseData
    {
        get => GetValue<AdvertiseData?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the scan response data sent when a scan request is received from a scanning device.
    /// </summary>
    public AdvertiseData? ScanResponseData
    {
        get => GetValue<AdvertiseData?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops BLE advertising and disposes of the GATT server callback proxy.
    /// </remarks>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        BluetoothLeAdvertiser.StopAdvertising(AdvertiseCallbackProxy);
        BluetoothGattServerCallbackProxy?.Dispose();
        BluetoothGattServerCallbackProxy = null;
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Gets or sets the advertise settings that are actually in effect after advertising starts.
    /// </summary>
    /// <remarks>
    /// These may differ from the requested settings if the system adjusts them.
    /// </remarks>
    public AdvertiseSettings? AdvertiseSettingsInEffect
    {
        get => GetValue<AdvertiseSettings?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Called when advertising starts successfully.
    /// </summary>
    /// <param name="settingsInEffect">The actual advertise settings in effect.</param>
    public void OnStartSuccess(AdvertiseSettings? settingsInEffect)
    {
        AdvertiseSettingsInEffect = settingsInEffect;
        OnStartSucceeded();
    }

    /// <summary>
    /// Called when advertising fails to start.
    /// </summary>
    /// <param name="errorCode">The error code indicating the failure reason.</param>
    /// <exception cref="AndroidNativeAdvertiseFailureException">Thrown when advertising fails to start.</exception>
    public void OnStartFailure(AdvertiseFailure errorCode)
    {
        try
        {
            throw new AndroidNativeAdvertiseFailureException(errorCode);
        }
        catch (Exception e)
        {
            OnStartFailed(e);
            BluetoothGattServerCallbackProxy?.Dispose();
            BluetoothGattServerCallbackProxy = null;
        }
    }
}
