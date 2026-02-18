using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastClientDevice : BaseBluetoothBroadcastClientDevice,
    BluetoothGattServerCallbackProxy.IDevice
{
    /// <summary>
    /// Gets or sets the native Android Bluetooth device.
    /// </summary>
    public Android.Bluetooth.BluetoothDevice? NativeDevice { get; private set; }

    /// <inheritdoc/>
    public BluetoothBroadcastClientDevice(IBluetoothBroadcaster broadcaster, IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest request) : base(broadcaster, request)
    {
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        NativeDevice = null;
        return ValueTask.CompletedTask;
    }

    // BluetoothGattServerCallbackProxy.IDevice implementation
    void BluetoothGattServerCallbackProxy.IDevice.OnMtuChanged(int mtu)
    {
        // MTU changed for this device
        // Can be used for optimizing data transfer
    }

    void BluetoothGattServerCallbackProxy.IDevice.OnExecuteWrite(int requestId, bool execute)
    {
        // Execute write request
        // This is part of the reliable write transaction
    }

    void BluetoothGattServerCallbackProxy.IDevice.OnNotificationSent(GattStatus status)
    {
        // Notification was sent to device
        // Can be used to track notification delivery
    }

    void BluetoothGattServerCallbackProxy.IDevice.OnPhyRead(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy)
    {
        // PHY settings read
    }

    void BluetoothGattServerCallbackProxy.IDevice.OnPhyUpdate(GattStatus status, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy)
    {
        // PHY settings updated
    }

    void BluetoothGattServerCallbackProxy.IDevice.OnConnectionStateChange(ProfileState status, ProfileState newState)
    {
        // Connection state changed
    }
}
