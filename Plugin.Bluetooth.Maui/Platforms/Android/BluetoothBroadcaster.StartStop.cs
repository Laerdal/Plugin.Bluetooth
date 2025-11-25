using Plugin.Bluetooth.Maui.PlatformSpecific;
using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = BluetoothAdapterProxy.BluetoothAdapter.ScanMode == Android.Bluetooth.ScanMode.ConnectableDiscoverable;
    }

    protected override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        BluetoothLeAdvertiserProxy.BluetoothLeAdvertiser.StartAdvertising(AdvertiseSettings, AdvertiseData, ScanResponseData, AdvertiseCallbackProxy);
        BluetoothGattServerCallbackProxy = new BluetoothGattServerCallbackProxy(this);
        return ValueTask.CompletedTask;
    }

    public AdvertiseSettings? AdvertiseSettings
    {
        get => GetValue<AdvertiseSettings?>(null);
        set => SetValue(value);
    }

    public AdvertiseData? AdvertiseData
    {
        get => GetValue<AdvertiseData?>(null);
        set => SetValue(value);
    }

    public AdvertiseData? ScanResponseData
    {
        get => GetValue<AdvertiseData?>(null);
        set => SetValue(value);
    }

    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        BluetoothLeAdvertiserProxy.BluetoothLeAdvertiser.StopAdvertising(AdvertiseCallbackProxy);
        BluetoothGattServerCallbackProxy?.Dispose();
        BluetoothGattServerCallbackProxy = null;
        return ValueTask.CompletedTask;
    }

    public AdvertiseSettings? AdvertiseSettingsInEffect
    {
        get => GetValue<AdvertiseSettings?>(null);
        set => SetValue(value);
    }

    public void OnStartSuccess(AdvertiseSettings? settingsInEffect)
    {
        AdvertiseSettingsInEffect = settingsInEffect;
        OnStartSucceeded();
    }

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
