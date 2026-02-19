using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalCharacteristic" />
public class BluetoothBroadcastCharacteristic : BaseBluetoothLocalCharacteristic,
                                                BluetoothGattServerCallbackProxy.IBluetoothGattCharacteristicDelegate
{
    /// <inheritdoc/>
    public BluetoothBroadcastCharacteristic(IBluetoothLocalService service, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec request, IBluetoothLocalDescriptorFactory descriptorFactory) : base(service, request, descriptorFactory)
    {
        throw new NotImplementedException("BluetoothBroadcastCharacteristic is not yet implemented on Android.");
    }

    /// <inheritdoc/>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Characteristic value updates are not yet implemented on Android.");
    }

    /// <inheritdoc/>
    public void OnCharacteristicReadRequest(
        BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate,
        int requestId,
        int offset)
    {
        throw new NotImplementedException("Characteristic read requests are not yet implemented on Android.");
    }

    /// <inheritdoc/>
    public void OnCharacteristicWriteRequest(
        BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate,
        int requestId,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[] value)
    {
        throw new NotImplementedException("Characteristic write requests are not yet implemented on Android.");
    }

    /// <inheritdoc/>
    public BluetoothGattServerCallbackProxy.IBluetoothGattDescriptorDelegate GetDescriptor(BluetoothGattDescriptor? native)
    {
        throw new NotImplementedException("Descriptors are not yet implemented on Android.");
    }

}
