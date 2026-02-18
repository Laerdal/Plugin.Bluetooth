using Android.Bluetooth;

using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastCharacteristic : BaseBluetoothBroadcastCharacteristic,
    BluetoothGattServerCallbackProxy.ICharacteristic
{
    /// <inheritdoc/>
    public BluetoothBroadcastCharacteristic(IBluetoothBroadcastService service, IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request) : base(service, request)
    {
        throw new NotImplementedException("BluetoothBroadcastCharacteristic is not yet implemented on Android.");
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    // BluetoothGattServerCallbackProxy.ICharacteristic implementation
    public void OnCharacteristicReadRequest(
        BluetoothGattServerCallbackProxy.IDevice sharedDevice,
        int requestId,
        int offset)
    {
        throw new NotImplementedException("Characteristic read requests are not yet implemented on Android.");
    }

    public void OnCharacteristicWriteRequest(
        BluetoothGattServerCallbackProxy.IDevice sharedDevice,
        int requestId,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[] value)
    {
        throw new NotImplementedException("Characteristic write requests are not yet implemented on Android.");
    }

    public void OnDescriptorReadRequest(
        BluetoothGattServerCallbackProxy.IDevice sharedDevice,
        int requestId,
        int offset,
        BluetoothGattDescriptor? descriptor)
    {
        throw  new NotImplementedException("Descriptors are not yet implemented on Android.");
    }

    public void OnDescriptorWriteRequest(
        BluetoothGattServerCallbackProxy.IDevice sharedDevice,
        int requestId,
        BluetoothGattDescriptor? descriptor,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[] value)
    {
        throw new NotImplementedException("Descriptors are not yet implemented on Android.");
    }
}
