using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc cref="BaseBluetoothConnectedDevice" />
public class AndroidBluetoothConnectedDevice : BaseBluetoothConnectedDevice,
                                               BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate
{
    /// <inheritdoc />
    public AndroidBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec) : base(broadcaster, spec)
    {
    }

    /// <summary>
    ///     Gets or sets the native Android Bluetooth device.
    /// </summary>
    public BluetoothDevice? NativeDevice { get; private set; }

    internal void SetNativeDevice(BluetoothDevice nativeDevice)
    {
        NativeDevice = nativeDevice ?? throw new ArgumentNullException(nameof(nativeDevice));
    }

    // BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate implementation
    void BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate.OnMtuChanged(int mtu)
    {
        OnMtuChanged(mtu);
    }

    void BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate.OnExecuteWrite(int requestId, bool execute)
    {
        // Execute write spec
        // This is part of the reliable write transaction
    }

    void BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate.OnNotificationSent(GattStatus status)
    {
        // Notification was sent to device
        // Can be used to track notification delivery
    }

    void BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate.OnPhyRead(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy)
    {
        // PHY settings read
    }

    void BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate.OnPhyUpdate(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy)
    {
        // PHY settings updated
    }

    /// <inheritdoc />
    public void OnConnectionStateChange(ProfileState status, ProfileState newState)
    {
        OnConnectionStatusChanged(newState == ProfileState.Connected);
    }

    /// <inheritdoc />
    protected override ValueTask DisposeAsyncCore()
    {
        NativeDevice = null;
        return base.DisposeAsyncCore();
    }


    /// <inheritdoc />
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var nativeDevice = NativeDevice ?? throw new InvalidOperationException("Native Android BluetoothDevice is not assigned for this connected device.");
        var androidBroadcaster = (AndroidBluetoothBroadcaster) Broadcaster;

        var gattServer = androidBroadcaster.GetGattServerOrDefault();
        if (gattServer == null)
        {
            throw new InvalidOperationException("Android GATT server is not available.");
        }

        gattServer.CancelConnection(nativeDevice);
        OnConnectionStatusChanged(false);
        return ValueTask.CompletedTask;
    }
}
