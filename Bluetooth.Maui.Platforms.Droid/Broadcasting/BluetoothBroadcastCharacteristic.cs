using Android.Bluetooth;

using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastCharacteristic : BaseBluetoothLocalCharacteristic,
    BluetoothGattServerCallbackProxy.IBluetoothGattCharacteristicDelegate
{
    /// <inheritdoc/>
    public BluetoothBroadcastCharacteristic(IBluetoothLocalService service, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec request, IBluetoothLocalDescriptorFactory descriptorFactory) : base(service, request, descriptorFactory)
    {
        throw new NotImplementedException("BluetoothBroadcastCharacteristic is not yet implemented on Android.");
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notify, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Characteristic value updates are not yet implemented on Android.");
    }

    // BluetoothGattServerCallbackProxy.IBluetoothGattCharacteristicDelegate implementation
    public void OnCharacteristicReadRequest(
        BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate sharedDevice,
        int requestId,
        int offset)
    {
        throw new NotImplementedException("Characteristic read requests are not yet implemented on Android.");
    }

    public void OnCharacteristicWriteRequest(
        BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate sharedDevice,
        int requestId,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[] value)
    {
        throw new NotImplementedException("Characteristic write requests are not yet implemented on Android.");
    }

    public BluetoothGattServerCallbackProxy.IBluetoothGattDescriptorDelegate GetDescriptor(BluetoothGattDescriptor? descriptor)
    {
        throw new NotImplementedException("Descriptors are not yet implemented on Android.");
    }
}
