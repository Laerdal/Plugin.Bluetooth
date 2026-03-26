using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalDescriptor" />
public class AndroidBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor,
                                              BluetoothGattServerCallbackProxy.IBluetoothGattDescriptorDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothLocalDescriptor" /> class.
    /// </summary>
    /// <param name="nativeDescriptor">The native Android descriptor represented by this instance.</param>
    /// <param name="characteristic">The characteristic that owns this descriptor.</param>
    /// <param name="id">The descriptor identifier.</param>
    /// <param name="initialValue">The optional initial descriptor value.</param>
    /// <param name="name">The optional descriptor name.</param>
    /// <param name="logger">The optional logger instance.</param>
    public AndroidBluetoothLocalDescriptor(BluetoothGattDescriptor nativeDescriptor,
        IBluetoothLocalCharacteristic characteristic,
        Guid id,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalDescriptor>? logger = null)
        : base(characteristic, id, initialValue, name, logger)
    {
        NativeDescriptor = nativeDescriptor ?? throw new ArgumentNullException(nameof(nativeDescriptor));
    }

    /// <summary>
    ///     Gets the native Android descriptor.
    /// </summary>
    public BluetoothGattDescriptor NativeDescriptor { get; }

    private AndroidBluetoothBroadcaster AndroidBroadcaster => (AndroidBluetoothBroadcaster) Characteristic.Service.Broadcaster;

    /// <inheritdoc />
    public void OnDescriptorReadRequest(BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate,
        int requestId,
        int offset,
        BluetoothGattDescriptor? descriptor)
    {
        ArgumentNullException.ThrowIfNull(sharedBluetoothDeviceDelegate);

        if (sharedBluetoothDeviceDelegate is not AndroidBluetoothConnectedDevice androidDevice || androidDevice.NativeDevice == null)
        {
            return;
        }

        try
        {
            var args = OnReadRequested(androidDevice, offset);
            var payload = args.ResponseValue?.ToArray() ?? [];
            AndroidBroadcaster.TrySendResponse(androidDevice.NativeDevice,
                                               requestId,
                                               GattStatus.Success,
                                               offset,
                                               payload);
        }
        catch (Exception)
        {
            AndroidBroadcaster.TrySendResponse(androidDevice.NativeDevice,
                                               requestId,
                                               GattStatus.RequestNotSupported,
                                               offset,
                                               []);
        }
    }

    /// <inheritdoc />
    public void OnDescriptorWriteRequest(BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate,
        int requestId,
        BluetoothGattDescriptor? descriptor,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[] value)
    {
        ArgumentNullException.ThrowIfNull(sharedBluetoothDeviceDelegate);
        ArgumentNullException.ThrowIfNull(value);

        if (sharedBluetoothDeviceDelegate is not AndroidBluetoothConnectedDevice androidDevice || androidDevice.NativeDevice == null)
        {
            return;
        }

        try
        {
            var args = OnWriteRequested(androidDevice,
                                        value,
                                        offset,
                                        preparedWrite,
                                        responseNeeded);

            if (responseNeeded)
            {
                AndroidBroadcaster.TrySendResponse(androidDevice.NativeDevice,
                                                   requestId,
                                                   GattStatus.Success,
                                                   offset,
                                                   []);
            }
        }
        catch (Exception)
        {
            if (responseNeeded)
            {
                AndroidBroadcaster.TrySendResponse(androidDevice.NativeDevice,
                                                   requestId,
                                                   GattStatus.RequestNotSupported,
                                                   offset,
                                                   []);
            }
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }
}