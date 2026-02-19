using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastClientDevice : BaseBluetoothConnectedDevice,
    BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate
{
    /// <summary>
    /// Gets or sets the native Android Bluetooth device.
    /// </summary>
    public Android.Bluetooth.BluetoothDevice? NativeDevice { get; private set; }

    /// <inheritdoc/>
    public BluetoothBroadcastClientDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec request) : base(broadcaster, request)
    {
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsyncCore()
    {
        NativeDevice = null;
        return base.DisposeAsyncCore();
    }


    /// <inheritdoc/>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Client device disconnect is not yet implemented on Android.");
    }

    // BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate implementation
    void BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate.OnMtuChanged(int mtu)
    {
        // MTU changed for this device
        // Can be used for optimizing data transfer
    }

    void BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate.OnExecuteWrite(int requestId, bool execute)
    {
        // Execute write request
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

    void BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate.OnConnectionStateChange(ProfileState status, ProfileState newState)
    {
        // Connection state changed
    }
}
